using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Helpers;
using NetCoreCalendar.Models;
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

        /** Checks the user role to see if we can fill the calendar with events and resources;
         * If the user is not logged in, creates empty lists of LessonVM and StudentVM for the calendar to load
         * and passes them to the corresponding ViewBags
         **/
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("User"))
            {
                var studentsVM = await studentRepository.GetAllStudentsVMAsync();
                var lessonsVM = await lessonRepository.GetAllLessonsForCalendarAsync();

                ViewData["Resources"] = JSONListHelper.GetResourceListJSONString(studentsVM);
                ViewData["Events"] = JSONListHelper.GetEventListJSONString(lessonsVM);
            }
            else
            {
                List<LessonVM> lessons = new List<LessonVM>();
                List<StudentVM> students = new List<StudentVM>();
                ViewData["Resources"] = JSONListHelper.GetResourceListJSONString(students);
                ViewData["Events"] = JSONListHelper.GetEventListJSONString(lessons);
            } 
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateVM model)
        {
            if (ModelState.IsValid)
            {
                // checks whether there is a lesson for the same day and time, false means that we can proceed
                if(await lessonRepository.ExistsDate(model) == false)
                {
                    await lessonRepository.CreateLesson(model);
                    return PartialView("_CreateLesson", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You have already had a lesson this day at this time");
                }
            }
            // fills the selectLists in case the model returns invalid, so that we can see our choice again
            await FillLists(model);
            return PartialView("_CreateLesson", model);
        }

        // GET
        [Authorize]
        /**  Fills the popUp modal with the students model,
         *   We initiate two selectLists to choose the student we need. Only users can see the popup modal.
         **/
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

        // GET
        /**  Fills the popUp modal with the information of the Student with the corresponding id;
         * We initiate two selectLists, in case we need to change the Student
         **/
        public async Task<PartialViewResult> Edit(int? id)
        {
            var model = await lessonRepository.GetLessonToUpdateAsync(id);
            var students = await studentRepository.GetAllStudentsAsync();
            if (model != null)
            {
                model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
                model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            }
            return PartialView("_Edit", model);
        }

        // POST: Home/Edit/5
        /** Updates the Lesson information
         **/
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
                // checks whether there is a lesson for the same day and time, false means that we can proceed
                if (await lessonRepository.ExistsDate(model) == false)
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
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You have already had a lesson this day at this time");
                }
            }
            // fills the selectLists in case the model returns invalid, so that we can see our choice again
            await FillLists(model);
            return PartialView("_Edit", model);
        }

        // POST
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

        /**
        * Fills the selectLists in case the model returns invalid, so that we can see our choice again
        **/
        private async Task FillLists(LessonCreateVM model)
        {
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
        }
    }
}