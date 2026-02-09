using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class CategoriaPacienteController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriaPacienteController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.CategoriaPaciente.ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaPaciente categoria)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _context.CategoriaPaciente.FindAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoriaPaciente categoria)
    {
        if (id != categoria.CategoriaPacienteId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.CategoriaPaciente.FindAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await _context.CategoriaPaciente.FindAsync(id);
        if (categoria != null)
        {
            _context.CategoriaPaciente.Remove(categoria);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}