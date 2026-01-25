using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Remover disciplina da base de dados
        /// </summary>
        /// <param name="id">id da Disciplina</param>
        public void Delete(uint id)
        {
            var disciplina = _context.Disciplinas.Find(id);
            if (disciplina != null)
            {
                _context.Remove(disciplina);
                _context.SaveChanges();
            }

        }

        /// <summary>
        /// Editar dados da disciplina
        /// </summary>
        /// <param name="disciplina">dados da disciplina</param>
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

        /// <summary>
        /// Buscar disciplina pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Dados da disciplina</returns>
        public Disciplina Get(uint id)
        {
            return _context.Disciplinas.FirstOrDefault(disciplina => disciplina.Id == id)
                ?? throw new ServiceException("Disciplina não encontrada.");
        }

        /// <summary>
        /// Buscar todas as disciplinas
        /// </summary>
        /// <returns>lista de disciplinas</returns>
        public IEnumerable<Disciplina> GetAll()
        {
            return _context.Disciplinas.AsNoTracking();
        }

        /// <summary>
        /// Obtém disciplinas pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IEnumerable<Disciplina> GetByNome(string nome)
        {
            IEnumerable<Disciplina> disciplinas = _context.Disciplinas
                .Where(disciplina => disciplina.Nome.StartsWith(nome))
                .AsNoTracking();
            return disciplinas;
        }
    }
}
