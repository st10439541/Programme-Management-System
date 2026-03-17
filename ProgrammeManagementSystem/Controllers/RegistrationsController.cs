using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegistrationsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var regs = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .OrderBy(r => r.Student!.LastName)
                .ToListAsync();
            return View(regs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.Registrations
                .Include(r => r.Student).Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);
            return reg == null ? NotFound() : View(reg);
        }

        public IActionResult Create()
        {
            ViewData["StudentID"] = new SelectList(_context.Students
                .OrderBy(s => s.LastName), "StudentID", "LastName");
            ViewData["ModuleID"] = new SelectList(_context.Modules
                .OrderBy(m => m.ModuleCode), "ModuleID", "ModuleName");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentID,ModuleID")] Registration registration)
        {
            // Prevent duplicate registration
            bool exists = await _context.Registrations
                .AnyAsync(r => r.StudentID == registration.StudentID && r.ModuleID == registration.ModuleID);
            if (exists)
            {
                ModelState.AddModelError("", "This student is already registered for that module.");
            }

            if (ModelState.IsValid)
            {
                registration.DateRegistered = DateTime.Now;
                _context.Add(registration);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student registered for module.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "StudentID", "LastName", registration.StudentID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleName", registration.ModuleID);
            return View(registration);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.Registrations
                .Include(r => r.Student).Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);
            return reg == null ? NotFound() : View(reg);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reg = await _context.Registrations.FindAsync(id);
            if (reg != null) { _context.Registrations.Remove(reg); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Registration removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
