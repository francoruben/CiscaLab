using CiscaLab.Data;
using CiscaLab.Models;
using CiscaLab.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CiscaLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResultadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Ingresar(int ordenTrabajoDetalleId)
        {
            var orden = await _context.OrdenTrabajoDetalle
                .Include(o => o.OrdenTrabajo)
                    .ThenInclude(ot => ot.Cita)
                        .ThenInclude(c => c.Paciente)
                .Include(o => o.OrdenTrabajo)
                    .ThenInclude(ot => ot.Cita)
                        .ThenInclude(c => c.Medico)
                            .ThenInclude(m => m.Especialidad)
                .Include(o => o.Examen)
                    .ThenInclude(e => e.ExamenesDetalle)
                        .ThenInclude(d => d.UnidadMedida)
                .Include(o => o.Examen)
                    .ThenInclude(e => e.ExamenesDetalle)
                        .ThenInclude(d => d.TipoResultado)
                .Include(o => o.Examen)
                    .ThenInclude(e => e.ExamenesDetalle)
                        .ThenInclude(d => d.ExamenesDetalleValoresNormales)
                .FirstOrDefaultAsync(o => o.OrdenTrabajoDetalleId == ordenTrabajoDetalleId);

                    

            if (orden == null || orden.OrdenTrabajo?.Cita.Paciente == null) 
                return NotFound();

            var paciente = orden.OrdenTrabajo.Cita.Paciente;
            var edad = DateTime.Today.Year - paciente.FechaNacimiento.Year;

            // Datos informativos para ViewBag
            ViewBag.NombrePaciente = $"{paciente.PrimerNombre} {paciente.SegundoNombre} {paciente.PrimerApellido} {paciente.SegundoApellido}";
            ViewBag.EdadPaciente = edad;
            ViewBag.SexoPaciente = paciente.Genero;


            var detallesVista = orden.Examen.ExamenesDetalle.Select(d => new ResultadoDetalleVistaViewModel
            {
                ExamenDetalleID = d.ExamenDetalleID,
                NombreValor = d.NombreValor,
                UnidadMedida = d.UnidadMedida?.NombreUnidadMedida,
                TipoResultado = d.TipoResultado?.NombreTipoResultado,
                ValorNormal = ObtenerValoresNormales(d.ExamenesDetalleValoresNormales.FirstOrDefault(), paciente.Genero, edad)
            }).ToList();

            ViewBag.DetallesVista = detallesVista;

            // Modelo limpio para el formulario
            var postModel = new IngresoResultadoPostViewModel
            {
                OrdenTrabajoDetalleId = ordenTrabajoDetalleId,
                Detalles = detallesVista.Select(d => new ResultadoDetallePostViewModel
                {
                    ExamenDetalleID = d.ExamenDetalleID
                }).ToList()
            };

            
            return View(postModel);

        }

        // Guardar Resultados
        [HttpPost]
        public async Task<IActionResult> Ingresar(IngresoResultadoPostViewModel model)
        {
            Console.WriteLine("POST recibido con " + model.Detalles?.Count + " detalles.");


            if (!ModelState.IsValid)
            {
                // Si hay errores, regresar a la vista con los datos informativos
                var orden = await _context.OrdenTrabajoDetalle
                    .Include(o => o.OrdenTrabajo)
                        .ThenInclude(ot => ot.Cita)
                            .ThenInclude(c => c.Paciente)
                    .Include(o => o.Examen)
                        .ThenInclude(e => e.ExamenesDetalle)
                            .ThenInclude(d => d.UnidadMedida)
                    .Include(o => o.Examen)
                        .ThenInclude(e => e.ExamenesDetalle)
                            .ThenInclude(d => d.TipoResultado)
                    .Include(o => o.Examen)
                        .ThenInclude(e => e.ExamenesDetalle)
                            .ThenInclude(d => d.ExamenesDetalleValoresNormales)
                    .FirstOrDefaultAsync(o => o.OrdenTrabajoDetalleId == model.OrdenTrabajoDetalleId);


                var paciente = orden.OrdenTrabajo.Cita.Paciente;
                var edad = DateTime.Today.Year - paciente.FechaNacimiento.Year;
                //if (paciente.FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

                ViewBag.NombrePaciente = $"{paciente.PrimerNombre} {paciente.SegundoNombre} {paciente.PrimerApellido} {paciente.SegundoApellido}";
                ViewBag.EdadPaciente = edad;
                ViewBag.SexoPaciente = paciente.Genero;

                var detallesVista = orden.Examen.ExamenesDetalle.Select(d => new ResultadoDetalleVistaViewModel
                {
                    ExamenDetalleID = d.ExamenDetalleID,
                    NombreValor = d.NombreValor,
                    UnidadMedida = d.UnidadMedida?.NombreUnidadMedida,
                    TipoResultado = d.TipoResultado?.NombreTipoResultado,
                    ValorNormal = ObtenerValoresNormales(d.ExamenesDetalleValoresNormales.FirstOrDefault(), paciente.Genero, edad)
                }).ToList();

                ViewBag.DetallesVista = detallesVista;

                foreach (var key in ModelState.Keys.Where(k => k.Contains("Observaciones")).ToList())
                {
                    ModelState[key].Errors.Clear();
                }

                return View(model);
            }

            //Guardar resultado
            var resultadoExamen = new ResultadoExamen
            {
                OrdenTrabajoDetalleId = model.OrdenTrabajoDetalleId,
                FechaRegistro = DateTime.Today,
                HoraRegistro = DateTime.Now,
                Observacion = model.Observacion,
                ResultadoExamenesDetalle = new List<ResultadoExamenDetalle>()
            };

            foreach (var detalle in model.Detalles)
            {
                var tipo = await _context.ExamenDetalle
                    .Where(e => e.ExamenDetalleID == detalle.ExamenDetalleID)
                    .Select(e => e.TipoResultadoId)
                    .FirstOrDefaultAsync();

                var resultadoDetalle = new ResultadoExamenDetalle
                {
                    ExamenDetalleId = detalle.ExamenDetalleID,
                    Observaciones = detalle.Observaciones
                };

                switch (tipo)
                {
                    case 1: //Numerico
                        resultadoDetalle.ValorResultadoNumero = decimal.TryParse(detalle.Resultado, out var num) ? num : 0;
                        break;
                    case 2: // Texto
                        resultadoDetalle.ValorResultadoTexto = detalle.Resultado;
                        break;
                    case 3: // Positivo/Negativo
                        resultadoDetalle.ValorResultadoPosNeg = detalle.Resultado?.ToLower() == "positivo";
                        break;

                }

                resultadoExamen.ResultadoExamenesDetalle.Add(resultadoDetalle);

               
            }

            _context.ResultadoExamen.Add(resultadoExamen);
            await _context.SaveChangesAsync();

            //Obtener la orden de trabajo
            var ordenTrabajoId = await _context.OrdenTrabajoDetalle
                .Where(d => d.OrdenTrabajoDetalleId == model.OrdenTrabajoDetalleId)
                .Select(d => d.OrdenTrabajoId)
                .FirstOrDefaultAsync();

            // Verificar si hay más exámenes pendientes
            var siguientePendiente = await _context.OrdenTrabajoDetalle
                .Where(d => d.OrdenTrabajoId == ordenTrabajoId &&
                            !_context.ResultadoExamen.Any(r => r.OrdenTrabajoDetalleId == d.OrdenTrabajoDetalleId))
                .Select(d => d.OrdenTrabajoDetalleId)
                .FirstOrDefaultAsync();

            


            if (siguientePendiente != 0)
            {
                // Aún hay exámenes pendientes → redirigir a SeleccionarExamenesPendientes
                return RedirectToAction("SeleccionarExamenesPendientes", new { ordenTrabajoId });
            }
            else
            {
                // Todos los exámenes fueron ingresados → actualizar estado
                var ordenTrabajo = await _context.OrdenTrabajo
                    .FirstOrDefaultAsync(o => o.OrdenTrabajoId == ordenTrabajoId);

                if (ordenTrabajo != null)
                {
                    ordenTrabajo.EstadoOrdenTrabajoId = 3; // Para Impresión
                    _context.Update(ordenTrabajo);
                    await _context.SaveChangesAsync();
                }

                // Redirigir a SeleccionarOrdenTrabajo
                return RedirectToAction("SeleccionarOrdenTrabajo");
            }


        }


        // Vista de Impresion
        public async Task<IActionResult> ResultadoFinal (int id)
        {
            var resultado = await _context.ResultadoExamen
                .Include(r => r.OrdenTrabajoDetalle)
                    .ThenInclude(o => o.OrdenTrabajo)
                        .ThenInclude(ot => ot.Cita)
                            .ThenInclude(c => c.Paciente)
                .Include(r => r.OrdenTrabajoDetalle)
                    .ThenInclude(o => o.OrdenTrabajo)
                        .ThenInclude(ot => ot.Cita)
                            .ThenInclude(c => c.Medico)
                                .ThenInclude(m => m.Especialidad)
                .Include(r => r.ResultadoExamenesDetalle)
                    .ThenInclude(rd => rd.ExamenDetalle)
                        .ThenInclude(ed => ed.UnidadMedida)
                .Include(r => r.ResultadoExamenesDetalle)
                    .ThenInclude(rd => rd.ExamenDetalle)
                        .ThenInclude(ed => ed.TipoResultado)
                .Include(r => r.ResultadoExamenesDetalle)
                    .ThenInclude(rd => rd.ExamenDetalle)
                        .ThenInclude(ed => ed.ExamenesDetalleValoresNormales)
                .FirstOrDefaultAsync(r => r.ResultadoExamenId == id);

            if (resultado == null)
                return NotFound();

            return View(resultado);
        }

        private string ObtenerValoresNormales(ExamenDetalleValoresNormales valores, string sexo, int edad)
        {
            if (valores == null) return "N/A";

            if (edad < 12)
                return $"{valores.VNInferiorNino}-{valores.VNSuperiorNino}";
            else if (sexo == "M")
                return $"{valores.VNInferiorHombre}-{valores.VNSuperiorHombre}";
            else if (sexo == "F")
                return $"{valores.VNInferiorMujer}-{valores.VNSuperiorMujer}";
            else
                return valores.VNUnico > 0 ? valores.VNUnico.ToString() : "N/A";
        }

        public async Task<IActionResult> SeleccionarOrden()
        {
            var ordenesPendientes = await _context.OrdenTrabajoDetalle
                .Include(o => o.OrdenTrabajo)
                    .ThenInclude(ot => ot.Cita)
                        .ThenInclude(c => c.Paciente)
                .Include(o => o.Examen)
                .Where(o => !_context.ResultadoExamen.Any(r => r.OrdenTrabajoDetalleId == o.OrdenTrabajoDetalleId))
                .ToListAsync();

            var viewModel = ordenesPendientes.Select(o => new OrdenSeleccionViewModel
            {
                OrdenTrabajoDetalleId = o.OrdenTrabajoDetalleId,
                NombrePaciente = $"{o.OrdenTrabajo.Cita.Paciente.PrimerNombre} {o.OrdenTrabajo.Cita.Paciente.PrimerApellido}",
                FechaCita = o.OrdenTrabajo.Cita.FechaCita,
                NombreExamen = o.Examen.NombreExamen
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> SeleccionarOrdenTrabajo()
        {
            var ordenes = await _context.OrdenTrabajo
                .Include(o => o.Cita)
                    .ThenInclude(c => c.Paciente)
                .Where(o => o.EstadoOrdenTrabajoId == 1 || o.EstadoOrdenTrabajoId == 2)
                .ToListAsync();

            var viewModel = ordenes.Select(o => new OrdenTrabajoSeleccionViewModel
            {
                OrdenTrabajoId = o.OrdenTrabajoId,
                NombrePaciente = $"{o.Cita.Paciente.PrimerNombre} {o.Cita.Paciente.PrimerApellido}",
                FechaCita = o.Cita.FechaCita,
                EstadoOrden = o.EstadoOrdenTrabajoId
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> SeleccionarExamenesPendientes(int ordenTrabajoId)
        {
            var examenesPendientes = await _context.OrdenTrabajoDetalle
                .Include(d => d.Examen)
                .Include(d => d.OrdenTrabajo)
                    .ThenInclude(o => o.Cita)
                        .ThenInclude(c => c.Paciente)
                .Where(d => d.OrdenTrabajoId == ordenTrabajoId &&
                            !_context.ResultadoExamen.Any(r => r.OrdenTrabajoDetalleId == d.OrdenTrabajoDetalleId))
                .ToListAsync();

            var viewModel = examenesPendientes.Select(d => new OrdenSeleccionViewModel
            {
                OrdenTrabajoDetalleId = d.OrdenTrabajoDetalleId,
                NombrePaciente = $"{d.OrdenTrabajo.Cita.Paciente.PrimerNombre} {d.OrdenTrabajo.Cita.Paciente.PrimerApellido}",
                FechaCita = d.OrdenTrabajo.Cita.FechaCita,
                NombreExamen = d.Examen.NombreExamen
            }).ToList();

            return View("SeleccionarExamenesPendientes", viewModel);
        }

        public async Task<IActionResult> OrdenesParaImpresion()
        {
            var ordenes = await _context.OrdenTrabajo
                .Include(o => o.Cita)
                    .ThenInclude(c => c.Paciente)
                .Where(o => o.EstadoOrdenTrabajoId == 3)
                .OrderByDescending(o => o.FechaOrdenTrabajo)
                .Select(o => new OrdenParaImpresionViewModel
                {
                    OrdenTrabajoId = o.OrdenTrabajoId,
                    Fecha = o.FechaOrdenTrabajo,
                    Paciente = o.Cita.Paciente.PrimerNombre + " " + o.Cita.Paciente.PrimerApellido,
                    Edad = DateTime.Today.Year - o.Cita.Paciente.FechaNacimiento.Year,
                    Sexo = o.Cita.Paciente.Genero
                })
                .ToListAsync();

            return View(ordenes);
        }

        public async Task<IActionResult> ReporteFinalPorOrden(int ordenTrabajoId)
        {
            var resultados = await _context.ResultadoExamen
                .Include(r => r.OrdenTrabajoDetalle)
                    .ThenInclude(d => d.Examen)
                        .ThenInclude(e => e.Metodo)
                .Include(r => r.ResultadoExamenesDetalle)
                    .ThenInclude(rd => rd.ExamenDetalle)
                        .ThenInclude(ed => ed.UnidadMedida)
                .Include(r => r.ResultadoExamenesDetalle)
                    .ThenInclude(rd => rd.ExamenDetalle)
                        .ThenInclude(ed => ed.ExamenesDetalleValoresNormales)
                .Where(r => r.OrdenTrabajoDetalle.OrdenTrabajoId == ordenTrabajoId)
                .ToListAsync();

            
            foreach (var r in resultados)
            {
                Console.WriteLine($"Examen: {r.OrdenTrabajoDetalle.Examen.NombreExamen}, Método: {r.OrdenTrabajoDetalle.Examen.Metodo?.NombreMetodo}");
            }


            var paciente = await _context.OrdenTrabajo
                .Include(o => o.Cita)
                    .ThenInclude(c => c.Paciente)
                .Where(o => o.OrdenTrabajoId == ordenTrabajoId)
                .Select(o => o.Cita.Paciente)
                .FirstOrDefaultAsync();

            var edad = DateTime.Today.Year - paciente.FechaNacimiento.Year;
            //if (paciente.FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;


            // 🔧 Proyectar resultados a List<Examen>
            var listaExamenes = resultados
                .GroupBy(r => r.OrdenTrabajoDetalle.Examen) // agrupo por el objeto Examen directamente
                .Select(g =>
                {
                    var examenPadre = g.Key;

                    return new Examen
                    {
                        ExamenId = examenPadre.ExamenId,
                        NombreExamen = examenPadre.NombreExamen,
                        Abreviatura = examenPadre.Abreviatura,
                        CategoriaExamenId = examenPadre.CategoriaExamenId,
                        Descripcion = examenPadre.Descripcion,
                        PrecioBase = examenPadre.PrecioBase,
                        ExcluirParaImprimir = examenPadre.ExcluirParaImprimir,
                        ImprimirEnUnaPagina = examenPadre.ImprimirEnUnaPagina,
                        CategoriaExamen = examenPadre.CategoriaExamen,
                        Metodo = examenPadre.Metodo,
                        // Los detalles del examen se arman desde los resultados

                        ResultadosDetalle = g.SelectMany(r => r.ResultadoExamenesDetalle).ToList()

                        //ExamenesDetalle = g.SelectMany(r => r.ResultadoExamenesDetalle)
                        //                   .Select(rd => rd.ExamenDetalle)
                        //                   .ToList()
                    };
                })
                .ToList();



            var reporte = new ReporteFinalViewModel
            {
                OrdenTrabajoId = ordenTrabajoId,
                NombrePaciente = $"{paciente.PrimerNombre} {paciente.SegundoNombre} {paciente.PrimerApellido} {paciente.SegundoApellido}",
                Edad = edad,
                Sexo = paciente.Genero,
                Fecha = DateTime.Today,
                Examenes = listaExamenes
            };

            return View(reporte);
        }


    }



}
