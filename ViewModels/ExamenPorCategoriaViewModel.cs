namespace CiscaLab.ViewModels
{
    public class ExamenPorCategoriaViewModel
    {
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; }
        public List<ExamenSeleccionable> Examenes { get; set; }

    }

    public class ExamenSeleccionable
    {
        public int ExamenId { get; set; }
        public string NombreExamen { get; set; }
        public bool Seleccionado { get; set; }

    }
}
