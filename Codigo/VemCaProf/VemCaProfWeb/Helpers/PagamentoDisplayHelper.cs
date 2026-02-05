using Core.Enums;

namespace VemCaProfWeb.Helpers
{
    public static class PagamentoDisplayHelper
    {
        public static string StatusLabel(string status) => status switch
        {
            StatusEnum.Agendada => "Agendada",
            StatusEnum.Realizada => "Realizada",
            StatusEnum.Paga => "Paga",
            StatusEnum.AguardandoPagamento => "Pendente",
            _ => status
        };

        public static string MetodoLabel(string metodo) => metodo switch
        {
            MetodoPagamentoEnum.Pix => "Pix",
            MetodoPagamentoEnum.Credito => "Crédito",
            MetodoPagamentoEnum.Debito => "Débito",
            _ => metodo
        };  
    }
}
