using Core;
using Core.DTO;
using Core.Enums;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
                    DataAula = c.DataHorarioInicio.Date,
                    Descricao = c.Descricao,
                    Status = c.Status,
                    Valor = c.Valor,
                    DataHoraPagamento = c.DataHoraPagamento,
                    MetodoPagamento = c.MetodoPagamento,
                    IdDisciplina = c.IdDisciplina,
                    IdResponsavel = c.IdResponsavel,
                    IdAluno = c.IdAluno,
                    IdProfessor = c.IdProfessor,
                    IdDisponibilidadeHorario = c.IdDisponibilidadeHorario ?? 0
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
                DataAula = aula.DataHorarioInicio.Date,
                Descricao = aula.Descricao,

                Status = aula.Status,
                Valor = aula.Valor,
                DataHoraPagamento = aula.DataHoraPagamento,
                MetodoPagamento = aula.MetodoPagamento,
                IdDisciplina = aula.IdDisciplina,
                IdResponsavel = aula.IdResponsavel,
                IdAluno = aula.IdAluno,
                IdProfessor = aula.IdProfessor,
                IdDisponibilidadeHorario = aula.IdDisponibilidadeHorario ?? 0
            };
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro ao buscar aula com ID {id}", ex);
        }
    }

    public IEnumerable<DisponibilidadeHorarioDTO> GetHorariosDisponiveis(int idProfessor, DateTime dataAula, int? idAula = null)
    {
        if (idProfessor <= 0 || dataAula == DateTime.MinValue)
            return Array.Empty<DisponibilidadeHorarioDTO>();

        var agora = DateTime.Now;
        var dataSelecionada = dataAula.Date;
        var aulaAtual = idAula.HasValue
            ? _context.Aulas.AsNoTracking().FirstOrDefault(a => a.Id == idAula && a.IdProfessor == idProfessor)
            : null;
        var aulasOcupadas = _context.Aulas
            .AsNoTracking()
            .Where(a => a.IdProfessor == idProfessor &&
                        a.Id != idAula &&
                        a.Status != StatusEnum.Cancelada)
            .Select(a => new { a.DataHorarioInicio, a.DataHorarioFinal })
            .ToList();

        return _context.DisponibilidadeHorarios
            .AsNoTracking()
            .Where(d => d.IdProfessor == idProfessor)
            .ToList()
            .GroupBy(d => new { d.HorarioInicio, d.HorarioFim })
            .Select(g => g.OrderBy(d => d.Id).First())
            .Select(d => new
            {
                Disponibilidade = d,
                Inicio = dataSelecionada.Add(d.HorarioInicio),
                Fim = dataSelecionada.Add(d.HorarioFim)
            })
            .Where(d => (dataSelecionada > agora.Date ||
                         d.Inicio > agora ||
                         (aulaAtual != null &&
                          (aulaAtual.IdDisponibilidadeHorario == d.Disponibilidade.Id ||
                           (aulaAtual.DataHorarioInicio == d.Inicio && aulaAtual.DataHorarioFinal == d.Fim)))) &&
                        d.Fim > d.Inicio &&
                        !aulasOcupadas.Any(a =>
                            a.DataHorarioInicio < d.Fim && a.DataHorarioFinal > d.Inicio))
            .OrderBy(d => d.Inicio)
            .Select(d => new DisponibilidadeHorarioDTO
            {
                Id = d.Disponibilidade.Id,
                Dia = dataSelecionada,
                HorarioInicio = d.Disponibilidade.HorarioInicio,
                HorarioFim = d.Disponibilidade.HorarioFim,
                IdProfessor = d.Disponibilidade.IdProfessor
            })
            .ToList();
    }

    /// <summary>
    /// Cadastra uma nova aula na base de dados
    /// </summary>
    /// <param name="aulaDto">dados da aula</param>
    /// <returns>id da aula</returns>
    public int Create(AulaDTO aulaDto)
    {
        using var transaction = aulaDto?.IdDisponibilidadeHorario > 0 && _context.Database.IsRelational()
            ? _context.Database.BeginTransaction(IsolationLevel.Serializable)
            : null;

        try
        {

            if (aulaDto == null)
                throw new ServiceException("Dados da aula não podem ser nulos");

            AplicarDisponibilidade(aulaDto);

            if (aulaDto.DataHorarioInicio == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");

            if (aulaDto.DataHorarioFinal == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");

            var descricao = aulaDto.Descricao?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ServiceException("A descrição da aula é obrigatório");



            aulaDto.Status = StatusEnum.Agendada;


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

            var aula = new Aula
            {
                DataHorarioInicio = aulaDto.DataHorarioInicio,
                DataHorarioFinal = aulaDto.DataHorarioFinal,
                Descricao = descricao,

                Status = aulaDto.Status,
                Valor = aulaDto.Valor,
                MetodoPagamento = null,
                IdDisciplina = aulaDto.IdDisciplina,
                IdResponsavel = aulaDto.IdResponsavel,
                IdAluno = aulaDto.IdAluno,
                IdProfessor = aulaDto.IdProfessor,
                IdDisponibilidadeHorario = aulaDto.IdDisponibilidadeHorario
            };


            _context.Aulas.Add(aula);
            _context.SaveChanges();
            transaction?.Commit();

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
        using var transaction = aulaDto?.IdDisponibilidadeHorario > 0 && _context.Database.IsRelational()
            ? _context.Database.BeginTransaction(IsolationLevel.Serializable)
            : null;

        try
        {
            if (aulaDto == null)
                throw new ServiceException("Dados da aula não podem ser nulos");

            AplicarDisponibilidade(aulaDto, aulaDto.Id);

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
            if (aula == null)
                return false;

            aula.DataHorarioInicio = aulaDto.DataHorarioInicio;
            aula.DataHorarioFinal = aulaDto.DataHorarioFinal;
            aula.Descricao = descricao;

            aula.Valor = aulaDto.Valor;
            aula.IdDisciplina = aulaDto.IdDisciplina;
            aula.IdResponsavel = aulaDto.IdResponsavel;
            aula.IdAluno = aulaDto.IdAluno;
            aula.IdProfessor = aulaDto.IdProfessor;
            aula.IdDisponibilidadeHorario = aulaDto.IdDisponibilidadeHorario;

            _context.Aulas.Update(aula);
            _context.SaveChanges();
            transaction?.Commit();

            return true;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Erro ao atualizar aula ID {aulaDto?.Id}", ex);
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

    /// <summary>
    /// Cancela Aula
    /// </summary>
    /// <param name="id">id da aula</param>
    /// <returns>true se for confirmada com sucesso</returns>
    public void CancelarAula(int id)
    {
        var aula = _context.Aulas.Find(id);

        if (aula == null)
            throw new ServiceException("Aula não encontrada.");

        if (aula.Status == StatusEnum.Realizada)
            throw new ServiceException("Não é possível cancelar uma aula que já foi realizada.");

        if (aula.Status == StatusEnum.Cancelada)
            throw new ServiceException("Esta aula já está cancelada.");

        if (aula.DataHorarioInicio.Date == DateTime.Now.Date)
        {
            throw new ServiceException("Não é permitido cancelar a aula no dia do agendamento. O cancelamento deve ser feito com antecedência.");
        }

        if (aula.DataHorarioInicio.Date < DateTime.Now.Date)
        {
            throw new ServiceException("Não é possível cancelar aulas de datas passadas.");
        }

        try 
        {
            aula.Status = StatusEnum.Cancelada;
            aula.IdDisponibilidadeHorario = null;
            _context.SaveChanges();
        }
        catch (ServiceException ex)
        {
            throw new ServiceException("Erro interno ao salvar o cancelamento da aula.", ex);
        }

    }

    /// <summary>
    /// Confirma a realização de uma aula
    /// </summary>
    /// <param name="id">id da aula</param>
    public void ConfirmarAula(int id)
    {
        var aula = _context.Aulas.Find(id);
    
        if (aula == null)
            throw new ServiceException("Aula não encontrada.");
    
        if (aula.Status == StatusEnum.Realizada)
            throw new ServiceException("Esta aula já foi realizada anteriormente.");
    
        if (aula.Status == StatusEnum.Confirmada)
            throw new ServiceException("Esta aula já está confirmada.");
    
        if (aula.Status == StatusEnum.Cancelada)
            throw new ServiceException("Não é possível confirmar uma aula que foi cancelada. É necessário reativá-la ou agendar uma nova.");
    
        if (aula.Status == StatusEnum.AguardandoPagamento || !aula.DataHoraPagamento.HasValue)
        {
            throw new ServiceException("Não é possível confirmar uma aula que está aguardando pagamento. Por favor, verifique o status financeiro primeiro.");
        }
        
        var agora = DateTime.Now;
        var horarioMinimoParaConfirmar = aula.DataHorarioInicio.AddHours(-2);
    
        if (agora < horarioMinimoParaConfirmar)
        {
            var tempoRestante = horarioMinimoParaConfirmar - agora;
            throw new ServiceException($"A confirmação só será permitida a partir das {horarioMinimoParaConfirmar:HH:mm}. (Faltam {tempoRestante.Hours}h {tempoRestante.Minutes}min)");
        }
    
        try 
        {
            aula.Status = StatusEnum.Confirmada; 
            _context.SaveChanges();
        }
        catch (ServiceException ex)
        {
            throw new ServiceException("Erro interno ao salvar a confirmação da aula.", ex);
        }
    }

    private void AplicarDisponibilidade(AulaDTO aulaDto, int? idAula = null)
    {
        if (aulaDto.IdDisponibilidadeHorario <= 0)
            return;

        if (aulaDto.DataAula == DateTime.MinValue)
            throw new ServiceException("Selecione a data da aula.");

        var disponibilidade = _context.DisponibilidadeHorarios
            .AsNoTracking()
            .FirstOrDefault(d => d.Id == aulaDto.IdDisponibilidadeHorario);

        if (disponibilidade == null || disponibilidade.IdProfessor != aulaDto.IdProfessor)
            throw new ServiceException("O horário selecionado não pertence ao professor informado.");

        var horarioDisponivel = GetHorariosDisponiveis(aulaDto.IdProfessor, aulaDto.DataAula, idAula)
            .Any(d => d.Id == disponibilidade.Id);

        if (!horarioDisponivel)
            throw new ServiceException("O horário selecionado não está mais disponível.");

        aulaDto.DataHorarioInicio = aulaDto.DataAula.Date.Add(disponibilidade.HorarioInicio);
        aulaDto.DataHorarioFinal = aulaDto.DataAula.Date.Add(disponibilidade.HorarioFim);
    }
}
