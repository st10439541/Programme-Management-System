using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModulesController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Modules.OrderBy(m => m.ModuleCode).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var module = await _context.Modules
                .Include(m => m.Registrations).ThenInclude(r => r.Student)
                .Include(m => m.ModuleAssignments).ThenInclude(a => a.Lecturer)
                .FirstOrDefaultAsync(m => m.ModuleID == id);
            return module == null ? NotFound() : View(module);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleName,ModuleCode,Credits,AcademicYear")] Module module)
        {
            if (!ModelState.IsValid) return View(module);
            _context.Add(module); await _context.SaveChangesAsync();
            TempData["Success"] = $"Module {module.ModuleCode} created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var m = await _context.Modules.FindAsync(id);
            return m == null ? NotFound() : View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleID,ModuleName,ModuleCode,Credits,AcademicYear")] Module module)
        {
            if (id != module.ModuleID) return NotFound();
            if (!ModelState.IsValid) return View(module);
            _context.Update(module); await _context.SaveChangesAsync();
            TempData["Success"] = "Module updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var m = await _context.Modules.FirstOrDefaultAsync(x => x.ModuleID == id);
            return m == null ? NotFound() : View(m);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var m = await _context.Modules.FindAsync(id);
            if (m != null) { _context.Modules.Remove(m); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Module deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
