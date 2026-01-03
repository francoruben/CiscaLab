using CiscaLab.Data;
using CiscaLab.Models;
using CiscaLab.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CiscaLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CitasController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Formulario de registro
        public async Task<IActionResult> Crear()
        {
            var viewModel = new CitaRegistroViewModel
            {
                FechaCita = DateTime.Today,
                Pacientes = await _context.Paciente
                    .Select(p => new SelectListItem
                    {
                        Value = p.PacienteID.ToString(),
                        Text = $"{p.PrimerNombre} {p.PrimerApellido}"
                    }).ToListAsync(),

                Medicos = await _context.Medico
                    .Select(m => new SelectListItem
                    {
                        Value = m.MedicoId.ToString(),
                        Text = m.NombreMedico
                    }).ToListAsync(),

                ExamenesDisponibles = await _context.Examen
                    .Select(e => new SelectListItem
                    {
                        Value = e.ExamenId.ToString(),
                        Text = e.NombreExamen
                    }).ToListAsync()
            };

            // Recuperar exámenes seleccionados desde TempData si vienen de la vista agrupada
            //var examenIdsRaw = TempData["ExamenesSeleccionados"]?.ToString();
            var examenIdsRaw = HttpContext.Session.GetString("ExamenesSeleccionados");

            if (!string.IsNullOrEmpty(examenIdsRaw))
            {
                viewModel.ExamenesSeleccionados = examenIdsRaw
                    .Split(',')
                    .Select(id => int.TryParse(id, out var parsed) ? parsed : 0)
                    .Where(id => id > 0)
                    .ToList();
            }



            return View(viewModel);
        }

        // POST: Guardar cita y generar orden de trabajo
        [HttpPost]
        public async Task<IActionResult> Crear(CitaRegistroViewModel model)
        {

            // Convertir los IDs de exámenes desde el campo oculto
            //var examenIdsRaw = Request.Form["ExamenesSeleccionados"];
            //if (!string.IsNullOrEmpty(examenIdsRaw))
            //{
            //    model.ExamenesSeleccionados = examenIdsRaw
            //        .ToString()
            //        .Split(',')
            //        .Select(id => int.TryParse(id, out var parsed) ? parsed : 0)
            //        .Where(id => id > 0)
            //        .ToList();

            //    // Guardar en sesión por si el usuario regresa
            //    HttpContext.Session.SetString("ExamenesSeleccionados", string.Join(",", model.ExamenesSeleccionados));

            //}

            // ✅ Verificar si no hay exámenes seleccionados
            if (model.ExamenesSeleccionados == null || !model.ExamenesSeleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un examen para la orden de trabajo.");

                // Recargar listas necesarias
                model.Medicos = await _context.Medico
                    .Select(m => new SelectListItem
                    {
                        Value = m.MedicoId.ToString(),
                        Text = m.NombreMedico
                    }).ToListAsync();

                model.ExamenesDisponibles = await _context.Examen
                    .Select(e => new SelectListItem
                    {
                        Value = e.ExamenId.ToString(),
                        Text = e.NombreExamen
                    }).ToListAsync();

                return View(model);
            }


            if (!ModelState.IsValid)
            {
                // Recargar listas si hay error
                model.Pacientes = await _context.Paciente
                    .Select(p => new SelectListItem
                    {
                        Value = p.PacienteID.ToString(),
                        Text = $"{p.PrimerNombre} {p.PrimerApellido}"
                    }).ToListAsync();
                model.Medicos = await _context.Medico
                    .Select(m => new SelectListItem
                    {
                        Value = m.MedicoId.ToString(),
                        Text = m.NombreMedico
                    }).ToListAsync();
                model.ExamenesDisponibles = await _context.Examen
                    .Select(e => new SelectListItem
                    {
                        Value = e.ExamenId.ToString(),
                        Text = e.NombreExamen
                    }).ToListAsync();
                return View(model);
            }

            var cita = new Cita
            {
                PacienteId = model.PacienteId,
                MedicoId = model.MedicoId,
                FechaCita = model.FechaCita
            };

            _context.Cita.Add(cita);
            await _context.SaveChangesAsync();

            var ordenTrabajo = new OrdenTrabajo
            {
                CitaId = cita.CitaId,
                EstadoOrdenTrabajoId = 1,
                FechaOrdenTrabajo = DateTime.Now
            };

            _context.OrdenTrabajo.Add(ordenTrabajo);
            await _context.SaveChangesAsync();

            foreach (var examenId in model.ExamenesSeleccionados)
            {
                var detalle = new OrdenTrabajoDetalle
                {
                    OrdenTrabajoId = ordenTrabajo.OrdenTrabajoId,
                    ExamenId = examenId
                };
                _context.OrdenTrabajoDetalle.Add(detalle);
            }

            await _context.SaveChangesAsync();

            // Limpiar sesión después de guardar
            HttpContext.Session.Remove("ExamenesSeleccionados");


            //return RedirectToAction("Confirmacion", new { id = cita.CitaId });
            return RedirectToAction("SeleccionarOrdenTrabajo", "Resultados", new { area = "Admin" });

        }

        public async Task<IActionResult> BuscarPaciente(string term)
        {
            var pacientes = await _context.Paciente
                .Where(p => (p.PrimerNombre + " " + p.PrimerApellido).Contains(term))
                .Select(p => new
                {
                    label = $"{p.PrimerNombre} {p.PrimerApellido}",
                    value = p.PacienteID
                })
                .ToListAsync();

            return Json(pacientes);
        }

        public async Task<IActionResult> BuscarExamen(string term)
        {
            var examenes = await _context.Examen
                .Where(e => e.NombreExamen.Contains(term))
                .Select(e => new
                {
                    label = e.NombreExamen,
                    value = e.ExamenId
                })
                .ToListAsync();

            return Json(examenes);
        }

        public async Task<IActionResult> SeleccionarExamenes()
        {

            var seleccion = HttpContext.Session.GetString("ExamenesSeleccionados");
            var seleccionIds = !string.IsNullOrEmpty(seleccion)
                ? seleccion.Split(',').Select(int.Parse).ToHashSet()
                : new HashSet<int>();


            var categorias = await _context.CategoriaExamen
                .Include(c => c.Examenes)
                .ToListAsync();

            var viewModel = categorias.Select(c => new ExamenPorCategoriaViewModel
            {
                CategoriaId = c.CategoriaExamenId,
                NombreCategoria = c.NombreCategoriaExamen,
                Examenes = c.Examenes.Select(e => new ExamenSeleccionable
                {
                    ExamenId = e.ExamenId,
                    NombreExamen = e.NombreExamen,
                    Seleccionado = seleccionIds.Contains(e.ExamenId)
                }).ToList()
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarExamenesSeleccionados(List<ExamenPorCategoriaViewModel> model)
        {
            var idsSeleccionados = model
                .SelectMany(c => c.Examenes)
                .Where(e => e.Seleccionado)
                .Select(e => e.ExamenId)
                .ToList();

            // Aquí puedes redirigir al flujo de creación de cita o guardar en sesión
            //TempData["ExamenesSeleccionados"] = string.Join(",", idsSeleccionados);
            HttpContext.Session.SetString("ExamenesSeleccionados", string.Join(",", idsSeleccionados));



            return RedirectToAction("Crear"); // o a una vista de confirmación
        }




    }
}