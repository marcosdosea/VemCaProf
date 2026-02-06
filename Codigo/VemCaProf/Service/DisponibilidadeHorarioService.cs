
using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service;

public class DisponibilidadeHorarioService : IDisponibilidadeHorarioService
{
    private readonly VemCaProfContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<DisponibilidadeHorarioService> _logger;

    public DisponibilidadeHorarioService(
        VemCaProfContext context,
        IMapper mapper,
        ILogger<DisponibilidadeHorarioService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public IEnumerable<DisponibilidadeHorarioDTO> GetAll()
    {
        try
        {
            var disponibilidadeHorario = _context.DisponibilidadeHorarios
                .AsNoTracking()
                .ToList();

            return _mapper.Map<IEnumerable<DisponibilidadeHorarioDTO>>(disponibilidadeHorario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar ");
            throw new ServiceException("Erro ao buscar ", ex);
        }
    }

    public DisponibilidadeHorarioDTO? Get(int id)
    {
        try
        {
            var disponibilidadeHorario = _context.DisponibilidadeHorarios
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            if (disponibilidadeHorario == null)
                return null;

            return _mapper.Map<DisponibilidadeHorarioDTO>(disponibilidadeHorario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar  ID {Id}", id);
            throw new ServiceException($"Erro ao buscar ID {id}", ex);
        }
    }




    public int Create(DisponibilidadeHorarioDTO disponibilidadeHorarioDto)
    {
        try
        {

            if (disponibilidadeHorarioDto.Dia == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");


            if (disponibilidadeHorarioDto.HorarioInicio == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");



            var disponibilidadeHorario = _mapper.Map<DisponibilidadeHorario>(disponibilidadeHorarioDto);


            _context.DisponibilidadeHorarios.Add(disponibilidadeHorario);
            _context.SaveChanges();

            return disponibilidadeHorario.Id;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar horário");
            throw new ServiceException("Erro ao criar horário", ex);
        }
    }


    public bool Update(DisponibilidadeHorarioDTO disponibilidadeHorarioDto)
    {
        try
        {

            if (disponibilidadeHorarioDto.Dia == DateTime.MinValue)
                throw new ServiceException("campo obrigatório");


            if (disponibilidadeHorarioDto.HorarioInicio == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");

            var disponibilidadeHorario = _context.DisponibilidadeHorarios.Find(disponibilidadeHorarioDto.Id);
            if (disponibilidadeHorario == null)
                return false;
            disponibilidadeHorario.HorarioInicio = disponibilidadeHorarioDto.HorarioInicio;
            disponibilidadeHorario.Dia = disponibilidadeHorarioDto.Dia;
            disponibilidadeHorario.HorarioFim = disponibilidadeHorarioDto.HorarioFim;
            disponibilidadeHorario.IdProfessor = disponibilidadeHorarioDto.IdProfessor;

            _context.DisponibilidadeHorarios.Update(disponibilidadeHorario);
            _context.SaveChanges();

            return true;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar  ID {Id}", disponibilidadeHorarioDto.Id);
            throw new ServiceException($"Erro ao atualizar  ID {disponibilidadeHorarioDto.Id}", ex);
        }
    }


    public bool Delete(int id)
    {
        try
        {
            var disponibilidadeHorario = _context.DisponibilidadeHorarios.Find(id);
            if (disponibilidadeHorario == null)
                return false;

            _context.DisponibilidadeHorarios.Remove(disponibilidadeHorario);
            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir  ID {Id}", id);
            throw new ServiceException($"Erro ao excluir ID {id}", ex);
        }
    }
}