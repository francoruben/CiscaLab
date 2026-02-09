using CiscaLab.Data;
using CiscaLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class MedicoController : Controller
{
    private readonly ApplicationDbContext _context;

    public MedicoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Medico
    public async Task<IActionResult> Index()
    {
        var medicos = _context.Medico.Include(m => m.Especialidad);
        return View(await medicos.ToListAsync());
    }

    // GET: Admin/Medico/Create
    public IActionResult Create()
    {
        ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "NombreEspecialidad");
        return View();
    }

    // POST: Admin/Medico/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Medico medico)
    {
        if (ModelState.IsValid)
        {
            medico.FechaRegistro = DateTime.Now;
            _context.Add(medico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "NombreEspecialidad", medico.EspecialidadId);
        return View(medico);
    }

    // GET: Admin/Medico/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var medico = await _context.Medico.FindAsync(id);
        if (medico == null) return NotFound();

        ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "NombreEspecialidad", medico.EspecialidadId);
        return View(medico);
    }

    // POST: Admin/Medico/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Medico medico)
    {
        if (id != medico.MedicoId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(medico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "EspecialidadId", "NombreEspecialidad", medico.EspecialidadId);
        return View(medico);
    }

    // GET: Admin/Medico/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var medico = await _context.Medico.Include(m => m.Especialidad)
                                           .FirstOrDefaultAsync(m => m.MedicoId == id);
        if (medico == null) return NotFound();
        return View(medico);
    }

    // POST: Admin/Medico/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var medico = await _context.Medico.FindAsync(id);
        if (medico != null)
        {
            _context.Medico.Remove(medico);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}