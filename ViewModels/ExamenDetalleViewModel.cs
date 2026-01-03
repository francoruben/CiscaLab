using CiscaLab.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CiscaLab.ViewModels
{
    public class ExamenDetalleViewModel
    {
        public ExamenDetalle Detalle { get; set; }
        public IEnumerable<SelectListItem> UnidadesMedida { get; set; }
        public IEnumerable<SelectListItem> TiposResultado { get; set; }

    }
}
