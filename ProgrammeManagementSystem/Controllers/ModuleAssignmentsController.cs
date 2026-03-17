using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class ModuleAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModuleAssignmentsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var assignments = await _context.ModuleAssignments
                .Include(a => a.Lecturer)
                .Include(a => a.Module)
                .OrderBy(a => a.Module!.ModuleCode)
                .ToListAsync();
            return View(assignments);
        }

        public IActionResult Create()
        {
            ViewData["LecturerID"] = new SelectList(_context.Lecturers
                .OrderBy(l => l.LastName), "LecturerID", "LastName");
            ViewData["ModuleID"] = new SelectList(_context.Modules
                .OrderBy(m => m.ModuleCode), "ModuleID", "ModuleName");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LecturerID,ModuleID")] ModuleAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Lecturer assigned to module.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["LecturerID"] = new SelectList(_context.Lecturers.OrderBy(l => l.LastName), "LecturerID", "LastName", assignment.LecturerID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleName", assignment.ModuleID);
            return View(assignment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var a = await _context.ModuleAssignments
                .Include(x => x.Lecturer).Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.AssignmentID == id);
            return a == null ? NotFound() : View(a);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var a = await _context.ModuleAssignments.FindAsync(id);
            if (a != null) { _context.ModuleAssignments.Remove(a); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Assignment removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
