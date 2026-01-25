using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    internal interface IDisciplinaService
    {
        uint Create(Disciplina disciplina);
        void Edit(Disciplina disciplina);
        void Delete(uint id);
        Disciplina Get(uint id);
        IEnumerable<Disciplina> GetAll();
        IEnumerable<Disciplina> GetByNome(string nome);

    }
}
