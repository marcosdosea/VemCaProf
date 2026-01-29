using System;

namespace Core.DTO
{
    public class DisponibilidadeHorarioDTO
    {
        public int Id { get; set; }
        public DateTime Dia { get; set; } 
        public TimeSpan HorarioInicio { get; set; }
        public TimeSpan HorarioFim { get; set; }
        public int IdProfessor { get; set; } 
    }
}
