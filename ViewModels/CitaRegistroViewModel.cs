using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CiscaLab.ViewModels
{
    public class CitaRegistroViewModel
    {
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public DateTime FechaCita { get; set; }
        [Required(ErrorMessage = "Seleccione al menos un examen")]
        public List<int> ExamenesSeleccionados { get; set; } = new();

        // Para mostrar en la vista
        public List<SelectListItem>? Pacientes { get; set; }
        public List<SelectListItem>? Medicos { get; set; }
        public List<SelectListItem>? ExamenesDisponibles { get; set; }
    }
}
