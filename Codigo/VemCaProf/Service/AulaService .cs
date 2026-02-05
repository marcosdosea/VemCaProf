using Core;
using Core.DTO;
using Core.Enums;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class AulaService : IAulaService
{
    private readonly VemCaProfContext _context;

    public AulaService(VemCaProfContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todas as aulas agendadas
    /// </summary>
    /// <returns>lista de aulas</returns>
    public IEnumerable<AulaDTO> GetAll()
    {
        try
        {
            return _context.Aulas
                .AsNoTracking()
                .Select(c => new AulaDTO
                {
                    Id = c.Id,
                    DataHorarioInicio = c.DataHorarioInicio,
                    DataHorarioFinal = c.DataHorarioFinal,
                    Descricao = c.Descricao,
                    
                    Status = c.Status,
                    Valor = c.Valor,
                    DataHoraPagamento = c.DataHoraPagamento,
                    MetodoPagamento = c.MetodoPagamento,
                    IdDisciplina = c.IdDisciplina,
                    IdResponsavel = c.IdResponsavel,
                    IdAluno = c.IdAluno,
                    IdProfessor = c.IdProfessor
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao buscar aula", ex);
        }
    }

    /// <summary>
    /// Busca uma aula pelo identificador
    /// </summary>
    /// <param name="id">id da aula</param>
    /// <returns>dados da aula</returns>
    public AulaDTO? Get(int id)
    {
        try
        {
            var aula = _context.Aulas
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            if (aula == null)
                return null;

            return new AulaDTO
            {
                Id = aula.Id,
                DataHorarioInicio = aula.DataHorarioInicio,
                DataHorarioFinal = aula.DataHorarioFinal,
                Descricao = aula.Descricao,
                
                Status = aula.Status,
                Valor = aula.Valor,
                DataHoraPagamento = aula.DataHoraPagamento,
                MetodoPagamento = aula.MetodoPagamento,
                IdDisciplina = aula.IdDisciplina,
                IdResponsavel = aula.IdResponsavel,
                IdAluno = aula.IdAluno,
                IdProfessor = aula.IdProfessor
            };
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro ao buscar aula com ID {id}", ex);
        }
    }

    /// <summary>
    /// Cadastra uma nova aula na base de dados
    /// </summary>
    /// <param name="aulaDto">dados da aula</param>
    /// <returns>id da aula</returns>
    public int Create(AulaDTO aulaDto)
    {
        try
        {
            
            if (aulaDto == null)
                throw new ServiceException("Dados da aula não podem ser nulos");

            if (aulaDto.DataHorarioInicio == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.DataHorarioFinal == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");
            
            var descricao = aulaDto.Descricao?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ServiceException("A descrição da aula é obrigatório");

            

            aulaDto.Status = StatusEnum.AguardandoPagamento;


            if (aulaDto.Valor <= 0)
                throw new ServiceException("Valor maior que 0");

            if (aulaDto.IdDisciplina <= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdResponsavel <= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdAluno<= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");

            var aula = new Aula
            {
                DataHorarioInicio = aulaDto.DataHorarioInicio,
                DataHorarioFinal = aulaDto.DataHorarioFinal,
                Descricao = descricao,
                
                Status = aulaDto.Status,
                Valor = aulaDto.Valor,
                MetodoPagamento=aulaDto.MetodoPagamento,
                IdDisciplina = aulaDto.IdDisciplina,
                IdResponsavel = aulaDto.IdResponsavel,
                IdAluno = aulaDto.IdAluno,
                IdProfessor = aulaDto.IdProfessor
            };
          

            _context.Aulas.Add(aula);
            _context.SaveChanges();

            return aula.Id;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao criar aula", ex);
        }
    }

    /// <summary>
    /// Atualiza os dados de uma aula existente
    /// </summary>
    /// <param name="aulaDto">dados atualizados da aula</param>
    /// <returns>true se atualizado com sucesso</returns>
    public bool Update(AulaDTO aulaDto)
    {
        try
        {
            if (aulaDto == null)
                throw new ServiceException("Dados da aula não podem ser nulos");

            if (aulaDto.DataHorarioInicio == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.DataHorarioFinal == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");

            var descricao = aulaDto.Descricao?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ServiceException("A descrição da aula é obrigatório");

            
            if (aulaDto.Valor <= 0)
                throw new ServiceException("Valor maior que 0");

            if (aulaDto.IdDisciplina <= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdResponsavel <= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdAluno <= 0)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");

            var aula = _context.Aulas.Find(aulaDto.Id);
            

                aula.DataHorarioInicio = aulaDto.DataHorarioInicio;
                aula.DataHorarioFinal = aulaDto.DataHorarioFinal;
                aula.Descricao = descricao;
                aula.Status = aulaDto.Status;
                
                aula.Valor = aulaDto.Valor;
                aula.MetodoPagamento = aulaDto.MetodoPagamento;
                aula.IdDisciplina = aulaDto.IdDisciplina;
                aula.IdResponsavel = aulaDto.IdResponsavel;
                aula.IdAluno = aulaDto.IdAluno;
                aula.IdProfessor = aulaDto.IdProfessor;

            _context.Aulas.Update(aula);
            _context.SaveChanges();

            return true;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro ao atualizar aula ID {aulaDto.Id}", ex);
        }
    }

    /// <summary>
    /// Remove uma aula da base de dados
    /// </summary>
    /// <param name="id">id da aula</param>
    /// <returns>true se removida com sucesso</returns>
    public bool Delete(int id)
    {
        try
        {
            var aula = _context.Aulas.Find(id);
            if (aula == null)
                return false;
            if (aula.Status != StatusEnum.AguardandoPagamento) 
                throw new ServiceException("Somente aulas com status 'Aguardando Pagamento' podem ser excluídas.");

            _context.Aulas.Remove(aula);
            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro ao excluir aula ID {id}", ex);
        }
    }
}