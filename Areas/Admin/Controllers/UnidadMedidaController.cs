using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class UnidadMedidaController : Controller
{
    private readonly ApplicationDbContext _context;

    public UnidadMedidaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/UnidadMedida
    public async Task<IActionResult> Index()
    {
        return View(await _context.UnidadMedida.ToListAsync());
    }

    // GET: Admin/UnidadMedida/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/UnidadMedida/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UnidadMedida unidad)
    {
        if (ModelState.IsValid)
        {
            _context.Add(unidad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(unidad);
    }

    // GET: Admin/UnidadMedida/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var unidad = await _context.UnidadMedida.FindAsync(id);
        if (unidad == null) return NotFound();
        return View(unidad);
    }

    // POST: Admin/UnidadMedida/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UnidadMedida unidad)
    {
        if (id != unidad.UnidadMedidaId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(unidad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(unidad);
    }

    // GET: Admin/UnidadMedida/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var unidad = await _context.UnidadMedida.FindAsync(id);
        if (unidad == null) return NotFound();
        return View(unidad);
    }

    // POST: Admin/UnidadMedida/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var unidad = await _context.UnidadMedida.FindAsync(id);
        if (unidad != null)
        {
            _context.UnidadMedida.Remove(unidad);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}