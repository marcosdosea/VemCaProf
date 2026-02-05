using System;

namespace Core.DTO
{
    public class AulaDTO
    {
        public int Id { get; set; }
        public DateTime DataHorarioInicio { get; set; }
        public DateTime DataHorarioFinal { get; set; }
        public string Descricao { get; set; } = null!;
        
        public string Status { get; set; } = null!;
        public Double Valor { get; set; }
        public DateTime DataHoraPagamento { get; set; }
        public string MetodoPagamento { get; set; } = null!; 
        public uint IdDisciplina { get; set; }
        public int IdResponsavel { get; set; }
        public int IdAluno { get; set; }
        public int IdProfessor { get; set; }
    }
}