using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class LecturersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LecturersController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Lecturers.OrderBy(l => l.LastName).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var lecturer = await _context.Lecturers
                .Include(l => l.ModuleAssignments).ThenInclude(a => a.Module)
                .FirstOrDefaultAsync(l => l.LecturerID == id);
            return lecturer == null ? NotFound() : View(lecturer);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Department")] Lecturer lecturer)
        {
            if (!ModelState.IsValid) return View(lecturer);
            _context.Add(lecturer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lecturer added.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var l = await _context.Lecturers.FindAsync(id);
            return l == null ? NotFound() : View(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LecturerID,FirstName,LastName,Email,Department")] Lecturer lecturer)
        {
            if (id != lecturer.LecturerID) return NotFound();
            if (!ModelState.IsValid) return View(lecturer);
            _context.Update(lecturer); await _context.SaveChangesAsync();
            TempData["Success"] = "Lecturer updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var l = await _context.Lecturers.FirstOrDefaultAsync(x => x.LecturerID == id);
            return l == null ? NotFound() : View(l);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var l = await _context.Lecturers.FindAsync(id);
            if (l != null) { _context.Lecturers.Remove(l); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Lecturer deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

