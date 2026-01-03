using CiscaLab.AccesoDatos.Data.Repository.IRepository;
using CiscaLab.Models;
using Microsoft.AspNetCore.Mvc;

namespace CiscaLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PacientesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PacientesController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create() 
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                //Logica para guardar en BD
                _contenedorTrabajo.Paciente.Add(paciente);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(paciente);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            Paciente paciente = new Paciente();
            paciente = _contenedorTrabajo.Paciente.Get(id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                //Logica para actualizar en BD
                _contenedorTrabajo.Paciente.Update(paciente);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(paciente);
        }


        #region Llamdas a la API

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Paciente.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            var objFromDB = _contenedorTrabajo.Paciente.Get(id);

            if (objFromDB == null)
            { 
                return Json(new {success = false, message = "Error borrando Paciente"});
            }

            _contenedorTrabajo.Paciente.Remove(objFromDB);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Paciente borrado correctamente" });


        }


        #endregion
    }
}
