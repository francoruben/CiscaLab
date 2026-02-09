using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class CategoriaExamenController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriaExamenController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/CategoriaExamen
    public async Task<IActionResult> Index()
    {
        return View(await _context.CategoriaExamen.ToListAsync());
    }

    // GET: Admin/CategoriaExamen/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/CategoriaExamen/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaExamen categoria)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    // GET: Admin/CategoriaExamen/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _context.CategoriaExamen.FindAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    // POST: Admin/CategoriaExamen/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoriaExamen categoria)
    {
        if (id != categoria.CategoriaExamenId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoria);
    }

    // GET: Admin/CategoriaExamen/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.CategoriaExamen.FindAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    // POST: Admin/CategoriaExamen/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await _context.CategoriaExamen.FindAsync(id);
        if (categoria != null)
        {
            _context.CategoriaExamen.Remove(categoria);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}