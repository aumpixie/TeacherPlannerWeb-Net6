using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;

        public StudentsController(ApplicationDbContext context, IStudentRepository studentRepository, IMapper mapper)
        {
            _context = context;
            this.studentRepository = studentRepository;
            this.mapper = mapper;
        }

        // GET: Students
        public async Task<IActionResult> Index()   
        {
            var students = await studentRepository.GetAllStudentsVMAsync();
              return _context.Students != null ? 
                          View(students) :
                          Problem("Entity set 'ApplicationDbContext.Students'  is null.");
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var model = await studentRepository.GetStudentAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateVM model)
        {
            if (ModelState.IsValid)
            {
                await studentRepository.CreateStudent(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Students/CreateForLesson
        /**
         * Returns the View of the specific View page that we can access only if we are creating
         * a Student object from the Lesson Create Page
         **/
        public IActionResult CreateForLesson()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForLesson(StudentCreateVM model)
        {
            if (ModelState.IsValid)
            {
                await studentRepository.CreateStudent(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await studentRepository.GetStudentToUpdateAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentCreateVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await studentRepository.UpdateStudentAsync(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await studentRepository.Exists(model.Id))
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
            return View(model);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            await studentRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
