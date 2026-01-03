using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscaLab.ViewModels
{
    public class ResultadoDetalleViewModel
    {
        public int ExamenDetalleID { get; set; }
        [BindNever]
        public string NombreValor { get; set; }
        [BindNever]
        public string UnidadMedida { get; set; }
        [BindNever]
        public string TipoResultado { get; set; }
        [BindNever]
        public string ValorNormal { get; set; }
        [Required(ErrorMessage = "El resultado es obligatorio")]
        public string Resultado { get; set; }
        public string Observaciones { get; set; }

    }
}
