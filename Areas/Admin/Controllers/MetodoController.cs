using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CiscaLab.Data;
using CiscaLab.Models;

[Area("Admin")]
public class MetodoController : Controller
{
    private readonly ApplicationDbContext _context;

    public MetodoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Metodo
    public async Task<IActionResult> Index()
    {
        return View(await _context.Metodo.ToListAsync());
    }

    // GET: Admin/Metodo/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Metodo/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Metodo metodo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(metodo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(metodo);
    }

    // GET: Admin/Metodo/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var metodo = await _context.Metodo.FindAsync(id);
        if (metodo == null) return NotFound();
        return View(metodo);
    }

    // POST: Admin/Metodo/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Metodo metodo)
    {
        if (id != metodo.MetodoId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(metodo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(metodo);
    }

    // GET: Admin/Metodo/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var metodo = await _context.Metodo.FindAsync(id);
        if (metodo == null) return NotFound();
        return View(metodo);
    }

    // POST: Admin/Metodo/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var metodo = await _context.Metodo.FindAsync(id);
        if (metodo != null)
        {
            _context.Metodo.Remove(metodo);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}