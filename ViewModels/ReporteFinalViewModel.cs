using CiscaLab.Models;

namespace CiscaLab.ViewModels
{
    public class ReporteFinalViewModel
    {
        public int OrdenTrabajoId { get; set; }
        public string NombrePaciente { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; }
        public DateTime Fecha { get; set; }

        //public Dictionary<string, List<ResultadoExamenDetalle>> Examenes { get; set; }
        public List<Examen> Examenes { get; set; }
    }

}
