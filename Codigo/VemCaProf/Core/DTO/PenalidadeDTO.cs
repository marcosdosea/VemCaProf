using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class PenalidadeDTO : PessoaDTO
    {
        public int id { get; set; }
        public DateTime dataHorario { get; set; }
        public string descricao { get; set; } = null!;
    }
}
