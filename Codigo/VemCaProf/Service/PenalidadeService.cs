using Core;
using Core.DTO;
using Core.Mappers;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PenalidadeService : IPenalidadeService
    {
        private readonly VemCaProfContext _context;

        public PenalidadeService(VemCaProfContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Criando uma nova penalidade na base de dados
        ///</summary>
        ///<param name="penalidade"></param>
        ///<returns> id do autor </returns>
        public int Create(PenalidadeDTO penalidade)
        {
           _context.Add(penalidade);
           _context.SaveChanges();

            return penalidade.Id;
           
        }

        /// <summary>
        /// Deletar uma penalidade pelo ID
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var penalidade = _context.Penalidades.Find(id);
            if (penalidade != null)
            {
               _context.Remove(penalidade);
               _context.SaveChanges();

            }
                
            
        }

        /// <summary>
        /// Editar penalidade existente
        /// </summary>
        /// <param name="penalidadeNova"></param>
        public void Edit(PenalidadeDTO penalidadeNova)
        {
            if(penalidadeNova.Id == 0)
                throw new ServiceException("ID da penalidade inválido.");

            var penalidadeExistente = _context.Penalidades.Find(penalidadeNova.Id);
            if (penalidadeExistente == null)
                throw new ServiceException("Penalidade não encontrada.");

            
            _context.Update(penalidadeNova);
            _context.SaveChanges();

        }


        /// <summary>
        ///   Buscar todas as penalidades
        /// </summary>
        /// <returns>Lista de penalidades</returns>
        public IEnumerable<Penalidade> GetAll()
        {
            var penalidade =  _context.Penalidades.AsNoTracking().ToList();
            return penalidade;

        }

        /// <summary>
        /// Buscar penalidade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Penalidade Get(int id)
        {
            var penalidade = _context.Penalidades.FirstOrDefault(p => p.Id == id);

            if (penalidade == null)
                return null;

            return penalidade;

        }

        /// <summary>
        /// Buscar penalidade pelo id da pessoa 
        /// </summary>
        /// <param name="pessoaId"></param>
        /// <returns></returns>
        public IEnumerable<Penalidade> GetByPessoaId(int pessoaId)
        {
            var penalidades = _context.Penalidades
                .AsNoTracking()
                .Where(p => p.Id == pessoaId);
            if(penalidades == null)
                throw new ServiceException("Nenhuma penalidade encontrada para essa pessoa.");


            return penalidades;

        }


    }
}
