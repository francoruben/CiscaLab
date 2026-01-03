namespace CiscaLab.ViewModels
{
    public class IngresoResultadoVistaViewModel
    {
        public int OrdenTrabajoDetalleId { get; set; }

        public string NombrePaciente { get; set; }
        public string SexoPaciente { get; set; }
        public int EdadPaciente { get; set; }

        public List<ResultadoDetalleVistaViewModel> Detalles { get; set; }
    }

}
