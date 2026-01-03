using System.ComponentModel.DataAnnotations;

namespace CiscaLab.ViewModels
{
    public class ResultadoDetallePostViewModel
    {
        public int ExamenDetalleID { get; set; }

        [Required(ErrorMessage = "Debe ingresar un resultado.")]
        public string Resultado { get; set; }

        public string Observaciones { get; set; }
    }

}
