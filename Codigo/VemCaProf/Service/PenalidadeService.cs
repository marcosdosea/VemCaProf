using Core;
using Core.DTO;
using Core.Mappers;
using Core.Service;

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
            if(penalidadeNova.Id == 0
                throw new ServiceException("ID da penalidade inválido.");

            var penalidadeExistente = _context.Penalidades.Find(penalidadeNova.Id);
            if (penalidadeExistente == null)
                throw new ServiceException("Penalidade não encontrada.");

            
            _context.Update(penalidadeNova);
            _context.SaveChanges();

        }

        public IEnumerable<PenalidadeDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PenalidadeDTO> GetByPessoaId(int pessoaId)
        {
            throw new NotImplementedException();
        }


    }
}
