namespace VemCaProfWeb.Models
{
    public class HomeViewModel
    {
        public int AlunosAtivos { get; set; }
        public int AulasSemana { get; set; }
        public int AulasHoje { get; set; }
        public decimal ReceitaMes { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
    }
}
