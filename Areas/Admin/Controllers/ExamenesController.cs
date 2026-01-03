using CiscaLab.Data;
using CiscaLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class ExamenesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExamenesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Examenes
    public async Task<IActionResult> Index()
    {
        var examenes = await _context.Examen
            .Include(e => e.CategoriaExamen)
            .Include(e => e.Metodo)
            .ToListAsync();

        return View(examenes);
    }

    // GET: Admin/Examenes/Create
    public IActionResult Create()
    {
        ViewBag.CategoriaExamenId = new SelectList(_context.CategoriaExamen.ToList(), "CategoriaExamenId", "NombreCategoriaExamen");
        ViewBag.MetodoId = new SelectList(_context.Metodo.ToList(), "MetodoId", "NombreMetodo");
        return View();
    }


    // POST: Admin/Examenes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Examen examen)
    {
        if (ModelState.IsValid)
        {
            _context.Add(examen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoriaExamenId = new SelectList(_context.CategoriaExamen.ToList(), "CategoriaExamenId", "NombreCategoriaExamen");
        ViewBag.MetodoId = new SelectList(_context.Metodo.ToList(), "MetodoId", "NombreMetodo");
        return View();
    }
}