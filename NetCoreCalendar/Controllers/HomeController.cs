using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Helpers;
using NetCoreCalendar.Models;
using NetCoreCalendar.Repositories;
using System.Diagnostics;

namespace NetCoreCalendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentRepository studentRepository;
        private readonly ILessonRepository lessonRepository;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger, IStudentRepository studentRepository,
            ILessonRepository lessonRepository, ApplicationDbContext context)
        {
            _logger = logger;
            this.studentRepository = studentRepository;
            this.lessonRepository = lessonRepository;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var studentsVM = await studentRepository.GetAllStudentsVMAsync();
                var lessonsVM = await lessonRepository.GetAllLessonsForCalendarAsync();
                if (studentsVM != null && lessonsVM != null)
                {
                    ViewData["Resources"] = JSONListHelper.GetResourceListJSONString(studentsVM);
                    ViewData["Events"] = JSONListHelper.GetEventListJSONString(lessonsVM);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please, log in to use the calendar");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateVM model)
        {
            if (ModelState.IsValid)
            {
                await lessonRepository.CreateLesson(model);
                return PartialView("_CreateLesson", model);
            }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return PartialView("_CreateLesson", model);
        }

        public async Task<PartialViewResult> ShowDialog()
        {
            var students = await studentRepository.GetAllStudentsAsync();
            var model = new LessonCreateVM
            {
                Students = new SelectList(students, "Id", "FirstName"),
                Rates = new SelectList(students, "Id", "Rate")
            };
            return PartialView("_CreateLesson", model);
        }

        public async Task<PartialViewResult> Edit(int? id)
        {
            var model = await lessonRepository.GetLessonToUpdateAsync(id);
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return PartialView("_Edit", model);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonCreateVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await lessonRepository.UpdateLessonAsync(model);
                    return PartialView("_Edit", model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return PartialView("_Edit", model);
            }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return PartialView("_Edit", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (context.Lessons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Lessons'  is null.");
            }
            await lessonRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        private bool LessonExists(int id)
        {
            return context.Lessons.Any(e => e.Id == id);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}