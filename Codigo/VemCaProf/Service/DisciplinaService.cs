using Core;
using Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class DisciplinaService : IDisciplinaService
    {
        private readonly VemCaProfContext _context;

        public DisciplinaService(VemCaProfContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criar uma nova disciplina na base de dados
        /// </summary>
        /// <param name="disciplina">dados da disciplina</param>
        /// <returns>id da disciplina</returns>
        public uint Create(Disciplina disciplina)
        {
            _context.Disciplinas.Add(disciplina);
            _context.SaveChanges();
            return (uint)disciplina.Id;
        }

        public void Delete(uint id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Editar dados da disciplina
        /// </summary>
        /// param name="disciplina">dados da disciplina</param>
        /// exception cref="ServiceException">lançada quando a disciplina é inválida ou não encontrada</exception>
        public void Edit(Disciplina disciplina)
        {
            if (disciplina == null || disciplina.Id == 0)
                throw new ServiceException("Disciplina inválida.");

            var disciplinaExistente = _context.Disciplinas.Find(disciplina.Id);
            if (disciplinaExistente == null)
                throw new ServiceException("Disciplina não encontrada.");
            _context.Update(disciplina);
            _context.SaveChanges();

        }

        public Disciplina Get(uint id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Disciplina> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Disciplina> GetByNome(string nome)
        {
            throw new NotImplementedException();
        }
    }
}
