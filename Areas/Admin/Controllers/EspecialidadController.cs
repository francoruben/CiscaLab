using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class EspecialidadController : Controller
{
    private readonly ApplicationDbContext _context;

    public EspecialidadController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Especialidad
    public async Task<IActionResult> Index()
    {
        return View(await _context.Especialidad.ToListAsync());
    }

    // GET: Admin/Especialidad/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Especialidad/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Especialidad especialidad)
    {
        if (ModelState.IsValid)
        {
            _context.Add(especialidad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(especialidad);
    }

    // GET: Admin/Especialidad/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var especialidad = await _context.Especialidad.FindAsync(id);
        if (especialidad == null) return NotFound();
        return View(especialidad);
    }

    // POST: Admin/Especialidad/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Especialidad especialidad)
    {
        if (id != especialidad.EspecialidadId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(especialidad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(especialidad);
    }

    // GET: Admin/Especialidad/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var especialidad = await _context.Especialidad.FindAsync(id);
        if (especialidad == null) return NotFound();
        return View(especialidad);
    }

    // POST: Admin/Especialidad/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var especialidad = await _context.Especialidad.FindAsync(id);
        if (especialidad != null)
        {
            _context.Especialidad.Remove(especialidad);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}