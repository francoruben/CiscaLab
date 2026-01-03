using CiscaLab.Data;
using CiscaLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiscaLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamenDetalleValoresNormalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamenDetalleValoresNormalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ExamenDetalleValoresNormales?detalleId=10
        public async Task<IActionResult> Index(int detalleId)
        {
            var valores = await _context.ExamenDetalleValoresNormales
                .Where(v => v.ExamenDetalleId == detalleId)
                .ToListAsync();

            ViewBag.Detalle = await _context.ExamenDetalle
                .Include(d => d.Examen)
                .FirstOrDefaultAsync(d => d.ExamenDetalleID == detalleId);

            return View(valores);
        }

        // GET: Admin/ExamenDetalleValoresNormales/Create
        public IActionResult Create(int detalleId)
        {
            var model = new ExamenDetalleValoresNormales
            {
                ExamenDetalleId = detalleId
            };

            return View(model);

        }
        // POST: Admin/ExamenDetalleValoresNormales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamenDetalleValoresNormales valores)
        {
            if (ModelState.IsValid)
            {
                _context.Add(valores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { detalleId = valores.ExamenDetalleId });
            }
            return View(valores);
        }

        // GET: Admin/ExamenDetalleValoresNormales/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var valores = await _context.ExamenDetalleValoresNormales.FindAsync(id);
            if (valores == null) return NotFound();
            return View(valores);
        }

        // POST: Admin/ExamenDetalleValoresNormales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamenDetalleValoresNormales valores)
        {
            if (id != valores.ExamenDetalleValoresNormalesId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(valores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { detalleId = valores.ExamenDetalleId });
            }
            return View(valores);
        }

        // GET: Admin/ExamenDetalleValoresNormales/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var valores = await _context.ExamenDetalleValoresNormales
                .Include(v => v.ExamenDetalle)
                .FirstOrDefaultAsync(v => v.ExamenDetalleValoresNormalesId == id);

            if (valores == null) return NotFound();
            return View(valores);
        }

        // POST: Admin/ExamenDetalleValoresNormales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valores = await _context.ExamenDetalleValoresNormales.FindAsync(id);
            if (valores != null)
            {
                _context.ExamenDetalleValoresNormales.Remove(valores);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { detalleId = valores.ExamenDetalleId });
        }




    }
}
