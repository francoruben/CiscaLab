using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class ClasificacionPacienteController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClasificacionPacienteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/ClasificacionPaciente
    public async Task<IActionResult> Index()
    {
        return View(await _context.ClasificacionPaciente.ToListAsync());
    }

    // GET: Admin/ClasificacionPaciente/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/ClasificacionPaciente/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClasificacionPaciente clasificacion)
    {
        if (ModelState.IsValid)
        {
            _context.Add(clasificacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(clasificacion);
    }

    // GET: Admin/ClasificacionPaciente/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var clasificacion = await _context.ClasificacionPaciente.FindAsync(id);
        if (clasificacion == null) return NotFound();
        return View(clasificacion);
    }

    // POST: Admin/ClasificacionPaciente/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClasificacionPaciente clasificacion)
    {
        if (id != clasificacion.ClasificacionPacienteId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(clasificacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(clasificacion);
    }

    // GET: Admin/ClasificacionPaciente/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var clasificacion = await _context.ClasificacionPaciente.FindAsync(id);
        if (clasificacion == null) return NotFound();
        return View(clasificacion);
    }

    // POST: Admin/ClasificacionPaciente/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var clasificacion = await _context.ClasificacionPaciente.FindAsync(id);
        if (clasificacion != null)
        {
            _context.ClasificacionPaciente.Remove(clasificacion);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}