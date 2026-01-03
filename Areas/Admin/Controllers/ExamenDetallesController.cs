using CiscaLab.Data;
using CiscaLab.Models;
using CiscaLab.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CiscaLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamenDetallesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamenDetallesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ExamenDetalles?examenId=5
        public async Task<IActionResult> Index(int examenId)
        {
            var detalles = await _context.ExamenDetalle
                .Include(d => d.UnidadMedida)
                .Include(d => d.TipoResultado)
                .Where(d => d.ExamenId == examenId)
                .ToListAsync();

            ViewBag.Examen = await _context.Examen.FindAsync(examenId);
            return View(detalles);
        }



        // GET: Admin/ExamenDetalles/Create
        public IActionResult Create(int examenId)
        {
            var vm = new ExamenDetalleViewModel
            {
                Detalle = new ExamenDetalle { ExamenId = examenId },
                UnidadesMedida = _context.UnidadMedida
                    .Select(u => new SelectListItem { Value = u.UnidadMedidaId.ToString(), Text = u.NombreUnidadMedida })
                    .ToList(),
                TiposResultado = _context.TipoResultado
                    .Select(t => new SelectListItem { Value = t.TipoResultadoId.ToString(), Text = t.NombreTipoResultado })
                    .ToList()
            };
            return View(vm);
        }





        // POST: Admin/ExamenDetalles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamenDetalle detalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { examenId = detalle.ExamenId });
            }

            // Recargar los combos si hay error de validación
            ViewBag.UnidadMedidaId = new SelectList(_context.UnidadMedida.ToList(), "UnidadMedidaId", "NombreUnidadMedida", detalle.UnidadMedidaId);
            ViewBag.TipoResultadoId = new SelectList(_context.TipoResultado.ToList(), "TipoResultadoId", "NombreTipoResultado", detalle.TipoResultadoId);
            ViewBag.ExamenId = detalle.ExamenId;

            return View(detalle);
        }


        // GET: Admin/ExamenDetalles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var detalle = await _context.ExamenDetalle.FindAsync(id);
            if (detalle == null) return NotFound();

            ViewData["UnidadMedidaId"] = new SelectList(_context.UnidadMedida, "UnidadMedidaId", "NombreUnidadMedida", detalle.UnidadMedidaId);
            ViewData["TipoResultadoId"] = new SelectList(_context.TipoResultado, "TipoResultadoId", "NombreTipoResultado", detalle.TipoResultadoId);
            return View(detalle);
        }

        // POST: Admin/ExamenDetalles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamenDetalle detalle)
        {
            if (id != detalle.ExamenDetalleID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(detalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { examenId = detalle.ExamenId });
            }
            return View(detalle);
        }

        // GET: Admin/ExamenDetalles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var detalle = await _context.ExamenDetalle
                .Include(d => d.Examen)
                .FirstOrDefaultAsync(d => d.ExamenDetalleID == id);

            if (detalle == null) return NotFound();
            return View(detalle);
        }

        // POST: Admin/ExamenDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle = await _context.ExamenDetalle.FindAsync(id);
            if (detalle != null)
            {
                _context.ExamenDetalle.Remove(detalle);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { examenId = detalle.ExamenId });
        }




    }
}
