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
        public int Create(PenalidadeDTO penalidadeDTO)
        {
            try
            {
                VerificarPessoaExistente(penalidadeDTO);
                VerificarData(penalidadeDTO);
                //TODO: verificar o limite de carecteres da descrição

                
                var penalidade = new Penalidade
                {
                    DataHorarioInicio = penalidadeDTO.DataHorarioInicio,
                    DataHoraFim = penalidadeDTO.DataHoraFim,
                    Tipo = penalidadeDTO.Tipo,
                    Descricao = penalidadeDTO.Descricao,
                    IdProfessor = penalidadeDTO.IdProfessor,
                    IdResponsavel = penalidadeDTO.IdResponsavel
                };

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
        /// <param name="penalidadeNovaDTO"></param>
        public void Edit(PenalidadeDTO penalidadeNovaDTO)
        {
         try{
                VerificarPessoaExistente(penalidadeNovaDTO);

                var penalidadeExistente = _context.Penalidades.Find(penalidadeNovaDTO.Id);
                 if (penalidadeExistente == null)
                throw new ServiceException("Penalidade não encontrada.");

                VerificarData(penalidadeNovaDTO);

                penalidadeExistente.DataHorarioInicio = penalidadeNovaDTO.DataHorarioInicio;
                penalidadeExistente.DataHoraFim = penalidadeNovaDTO.DataHoraFim;
                penalidadeExistente.Tipo = penalidadeNovaDTO.Tipo;
                penalidadeExistente.Descricao = penalidadeNovaDTO.Descricao;
                penalidadeExistente.IdProfessor = penalidadeNovaDTO.IdProfessor;
                penalidadeExistente.IdResponsavel = penalidadeNovaDTO.IdResponsavel;
                penalidadeExistente.NomeProfessor = penalidadeNovaDTO.NomeProfessor;
                penalidadeExistente.NomeResponsavel = penalidadeNovaDTO.NomeResponsavel;

                _context.Penalidades.Update(penalidadeExistente);
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
        public IEnumerable<PenalidadeDTO> GetAll()
        {
            var penalidades = _context.Penalidades
                .Include(p => p.IdProfessorNavigation)
                .Include(p => p.IdResponsavelNavigation)
                .ToList();

            return penalidades.Select(p => new PenalidadeDTO
            {
                Id = p.Id,
                DataHorarioInicio = p.DataHorarioInicio,
                DataHoraFim = p.DataHoraFim ?? DateTime.MinValue,
                Tipo = p.Tipo,
                Descricao = p.Descricao,
                IdProfessor = p.IdProfessor,
                IdResponsavel = p.IdResponsavel,
                NomeProfessor = p.IdProfessorNavigation?.Nome ?? string.Empty,
                NomeResponsavel = p.IdResponsavelNavigation?.Nome ?? string.Empty
            }).ToList();

        }

        /// <summary>
        /// Buscar penalidade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PenalidadeDTO? Get(int id)
        {
            var penalidade = _context.Penalidades.Find(id);
            if (penalidade == null)
                return null;

            return returnPenalidadeDTO(penalidade);
        }

        // <summary>
        /// Verificar se penalidade é nula, se professor ou responsável existem
        /// </summary>
        /// <param name="penalidade"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>"
        public void VerificarPessoaExistente(PenalidadeDTO penalidade)
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
        public void VerificarData(PenalidadeDTO penalidade)
        {
            if (penalidade.DataHorarioInicio >= penalidade.DataHoraFim)
                throw new ServiceException("Data de início deve ser anterior à data de fim.");
        }

        /// <summary>
        /// Buscar penalidade pelo id da pessoa 
        /// </summary>
        /// <param name="pessoaId"></param>
        /// <returns></returns>
        public IEnumerable<PenalidadeDTO> GetByPessoaId(int pessoaId)
        {
            var penalidade = _context.Penalidades
                .AsNoTracking()
                .Where(p => p.IdProfessor == pessoaId || p.IdResponsavel == pessoaId);
            if(!penalidade.Any())
                throw new ServiceException("Nenhuma penalidade encontrada para essa pessoa.");

            return penalidade.Select(p => new PenalidadeDTO
            {
                Id = p.Id,
                DataHorarioInicio = p.DataHorarioInicio,
                DataHoraFim = p.DataHoraFim ?? DateTime.MinValue,
                Tipo = p.Tipo,
                Descricao = p.Descricao,
                IdProfessor = p.IdProfessor,
                IdResponsavel = p.IdResponsavel,
                NomeProfessor = p.NomeProfessor,
                NomeResponsavel = p.NomeResponsavel
            }).ToList();

        }

        public PenalidadeDTO  returnPenalidadeDTO(Penalidade penalidade)
        {

            return new PenalidadeDTO
            {
                Id = penalidade.Id,
                DataHorarioInicio = penalidade.DataHorarioInicio,
                DataHoraFim = penalidade.DataHoraFim ?? DateTime.MinValue,
                Tipo = penalidade.Tipo,
                Descricao = penalidade.Descricao,
                IdProfessor = penalidade.IdProfessor,
                IdResponsavel = penalidade.IdResponsavel,
                NomeProfessor = penalidade.NomeProfessor,
                NomeResponsavel = penalidade.NomeResponsavel
            };
        }


     }
}
