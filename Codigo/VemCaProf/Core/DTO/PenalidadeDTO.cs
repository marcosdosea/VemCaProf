using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class PenalidadeDTO : PessoaDTO
    {
        public int Id { get; set; }
        public DateTime DataHorarioInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string Tipo { get; set; } = null!;
        public string Descricao { get; set; } = null!;

        public int IdProfessor { get; set; }

        public int IdResponsavel { get; set; }

    }
}
