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
        private readonly IPessoaService _pessoaService;


        public PenalidadeService(VemCaProfContext context, IPessoaService pessoaService)
        {
            _context = context;
            _pessoaService = pessoaService;
        }

        /// <summary>
        /// Criando uma nova penalidade na base de dados
        ///</summary>
        ///<param name="penalidade"></param>
        ///<returns> id do autor </returns>
        public int Create(Penalidade penalidade)
        {
            try
            {
                VerificarPessoaExistente(penalidade);
                VerificarData(penalidade);
                //TODO: verificar o limite de carecteres da descrição
                _context.Penalidades.Add(penalidade);
                _context.SaveChanges();

                return penalidade.Id;
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Erro ao criar penalidade", ex);
            }


        }

        /// <summary>
        /// Deletar uma penalidade pelo ID
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var penalidade = _context.Penalidades.Find(id);
            if (penalidade == null)
                throw new ServiceException("Penalidade não encontrada.");

            _context.Penalidades.Remove(penalidade);
            _context.SaveChanges();

            
                
            
        }

        /// <summary>
        /// Editar penalidade existente
        /// </summary>
        /// <param name="penalidadeNova"></param>
        public void Edit(Penalidade penalidadeNova)
        {
         try{
                VerificarPessoaExistente(penalidadeNova);
                var penalidadeExistente = Get(penalidadeNova.Id);
                 if (penalidadeExistente == null)
                throw new ServiceException("Penalidade não encontrada.");
                VerificarData(penalidadeNova);

                penalidadeExistente.DataHorarioInicio = penalidadeNova.DataHorarioInicio;
                penalidadeExistente.DataHoraFim = penalidadeNova.DataHoraFim;
                penalidadeExistente.Tipo = penalidadeNova.Tipo;
                penalidadeExistente.Descricao = penalidadeNova.Descricao;
                penalidadeExistente.IdProfessor = penalidadeNova.IdProfessor;
                penalidadeExistente.IdResponsavel = penalidadeNova.IdResponsavel;
           
                _context.SaveChanges();
          }
          catch (ServiceException)
          {
               throw;
          }

        }


        /// <summary>
        ///   Buscar todas as penalidades
        /// </summary>
        /// <returns>Lista de penalidades</returns>
        public IEnumerable<Penalidade> GetAll()
        {
            return _context.Penalidades
                .Include(p => p.IdProfessorNavigation)
                .Include(p => p.IdResponsavelNavigation)
                .ToList();

        }

        /// <summary>
        /// Buscar penalidade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Penalidade Get(int id)
        {
            return _context.Penalidades.Find(id);

        }

        // <summary>
        /// Verificar se penalidade é nula, se professor ou responsável existem
        /// </summary>
        /// <param name="penalidade"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>"
        public void VerificarPessoaExistente(Penalidade penalidade)
        {
            if (penalidade == null)
                throw new ServiceException("Os dados não podem ser nulos");

            var professor = _pessoaService.Get(penalidade.IdProfessor);
            if (professor == null || professor.TipoPessoa != "P")
                throw new ServiceException($"Professor de ID {penalidade.IdProfessor} não existe");

            var responsavel = _pessoaService.Get(penalidade.IdResponsavel);
            if (responsavel == null || responsavel.TipoPessoa != "R")
                throw new ServiceException($"Responsável de ID {penalidade.IdResponsavel} não existe");
        }

        //<sumary>
        ///verifica data de início e fim da penalidade
        ///</sumary>
        ///<param name="penalidade"></param>
        ///<returns></returns>
        ///<exception cref="ServiceException"></exception>
        public void VerificarData(Penalidade penalidade)
        {
            if (penalidade.DataHorarioInicio >= penalidade.DataHoraFim)
                throw new ServiceException("Data de início deve ser anterior à data de fim.");
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
                .Where(p => p.IdProfessor == pessoaId || p.IdResponsavel == pessoaId);
            if(!penalidades.Any())
                throw new ServiceException("Nenhuma penalidade encontrada para essa pessoa.");


            return penalidades;

        }


    }
}
