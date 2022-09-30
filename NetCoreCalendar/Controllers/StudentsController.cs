using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var students = mapper.Map<List<StudentVM>>(await studentRepository.GetAllAsync());
              return _context.Students != null ? 
                          View(students) :
                          Problem("Entity set 'ApplicationDbContext.Students'  is null.");
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var student = await studentRepository.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var studentVM = mapper.Map<StudentVM>(student);
            return View(studentVM);
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
        public async Task<IActionResult> Create(StudentVM studentVM)
        {
            if (ModelState.IsValid)
            {
                var student = mapper.Map<Student>(studentVM);
                await studentRepository.AddAsync(student);
                return RedirectToAction(nameof(Index));
            }
            return View(studentVM);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var student = await studentRepository.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var studentVM = mapper.Map<StudentVM>(student);
            return View(studentVM);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentVM studentVM)
        {
            if (id != studentVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = mapper.Map<Student>(studentVM);
                    await studentRepository.UpdateAsync(student);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await studentRepository.Exists(studentVM.Id))
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
            return View(studentVM);
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
