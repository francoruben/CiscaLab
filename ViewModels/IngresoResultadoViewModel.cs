using CiscaLab.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscaLab.ViewModels
{
    public class IngresoResultadoViewModel
    {
        public int OrdenTrabajoDetalleId { get; set; }
        public string Observacion { get; set; }
        [BindNever]
        //public string SexoPaciente { get; set; }
        //public int EdadPaciente { get; set; }
        //[BindNever]
        //public string NombrePaciente { get; set; }
        public List<ResultadoDetallePostViewModel> Detalles { get; set; }


    }
}
