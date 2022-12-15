using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;
using NetCoreCalendar.Repositories;
using Org.BouncyCastle.Bcpg;

namespace NetCoreCalendar.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<Teacher> userManager;
        private readonly IStudentRepository studentRepository;
        private readonly ILessonRepository lessonRepository;

        public LessonsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
            UserManager<Teacher> userManager, IStudentRepository studentRepository, ILessonRepository lessonRepository)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.studentRepository = studentRepository;
            this.lessonRepository = lessonRepository;
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            var model = await lessonRepository.GetMyLessonsAsync();
            return View(model);
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var model = await lessonRepository.GetLessonAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }


        // GET: Lessons/Create
        public async Task<IActionResult> Create()
        {
            var students = await studentRepository.GetAllStudentsAsync();
            var model = new LessonCreateVM
            {
                Students = new SelectList(students, "Id", "FirstName"),
                StartDate = DateTime.Today,
                Rates = new SelectList(students, "Id", "Rate")
            };
            return View(model);
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateVM model)
        {
           if (ModelState.IsValid)
           {
                    await lessonRepository.CreateLesson(model);
                    return RedirectToAction(nameof(Index));
           }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return View(model);
        }

        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await lessonRepository.GetLessonToUpdateAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return View(model);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                return RedirectToAction(nameof(Index));
            }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            model.Rates = new SelectList(students, "Id", "Rate", model.StudentId);
            return View(model);
        }


        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Lessons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Lessons'  is null.");
            }
            await lessonRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("MarkPaid")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            await lessonRepository.UpdatePaid(id);
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
          return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
