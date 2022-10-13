using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
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

        public IActionResult Index()
        {
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
                return PartialView("Dummy", model);
            }
            var students = await studentRepository.GetAllStudentsAsync();
            model.Students = new SelectList(students, "Id", "FirstName", model.StudentId);
            return PartialView("Dummy", model);
        }

        public async Task<PartialViewResult> ShowDialog()
        {
            var students = await studentRepository.GetAllStudentsAsync();
            var model = new LessonCreateVM
            {
                Students = new SelectList(students, "Id", "FirstName"),
            };
            return PartialView("Dummy", model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}