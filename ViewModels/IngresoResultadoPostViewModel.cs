namespace CiscaLab.ViewModels
{
    public class IngresoResultadoPostViewModel
    {
        public int OrdenTrabajoDetalleId { get; set; }

        public string Observacion { get; set; }

        public List<ResultadoDetallePostViewModel> Detalles { get; set; }
    }

}
