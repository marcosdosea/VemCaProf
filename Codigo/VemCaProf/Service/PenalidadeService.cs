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
        public int Create(PenalidadeDTO penalidade)
        {
            try
            {
                if (penalidade == null)
                    throw new ServiceException("Os dados não podem ser nulos");


                var data = penalidade.DataHorarioInicio;
                var descricao = penalidade.Descricao;


                if (penalidade.DataHorarioInicio == default(DateTime))
                    throw new ServiceException("A data é obrigatória");

                if (string.IsNullOrWhiteSpace(descricao))
                    throw new ServiceException("A descrição é obrigatória");


                if (_pessoaService.GetProfessor(penalidade.IdProfessor) == null)
                    throw new ServiceException($"Professor de ID {penalidade.IdProfessor} não existe");

                if (_pessoaService.GetResponsavel(penalidade.IdResponsavel) == null)
                    throw new ServiceException($"Responsável de ID {penalidade.IdResponsavel} não existe");

                var entity = new Penalidade
                {
                    DataHorarioInicio = penalidade.DataHorarioInicio,
                    DataHoraFim = penalidade.DataHoraFim,
                    Tipo = penalidade.Tipo,
                    Descricao = penalidade.Descricao,
                    IdProfessor = penalidade.IdProfessor,
                    IdResponsavel = penalidade.IdResponsavel
                };

                _context.Penalidades.Add(entity);
                _context.SaveChanges();

                return entity.Id;
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Erro ao criar cidade", ex);
            }


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
            if (penalidadeNova.DataHorarioInicio == default(DateTime))
                throw new ServiceException("A data é obrigatória");

            if (string.IsNullOrWhiteSpace(penalidadeNova.Descricao))
                throw new ServiceException("A descrição é obrigatória");


            if (_pessoaService.GetProfessor(penalidadeNova.IdProfessor) == null)
                throw new ServiceException($"Professor de ID {penalidadeNova.IdProfessor} não existe");

            if (_pessoaService.GetResponsavel(penalidadeNova.IdResponsavel) == null)
                throw new ServiceException($"Responsável de ID {penalidadeNova.IdResponsavel} não existe");

            var penalidadeExistente = _context.Penalidades.Find(penalidadeNova.Id);
            if (penalidadeExistente == null)
                throw new ServiceException("Penalidade não encontrada.");

            penalidadeExistente.DataHorarioInicio = penalidadeNova.DataHorarioInicio;
            penalidadeExistente.DataHoraFim = penalidadeNova.DataHoraFim;
            penalidadeExistente.Tipo = penalidadeNova.Tipo;
            penalidadeExistente.Descricao = penalidadeNova.Descricao;
            penalidadeExistente.IdProfessor = penalidadeNova.IdProfessor;
            penalidadeExistente.IdResponsavel = penalidadeNova.IdResponsavel;
            
            _context.Update(penalidadeExistente);
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
