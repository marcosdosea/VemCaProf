
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
   

    public DisponibilidadeHorarioService(
        VemCaProfContext context)
    {
        _context = context;
        
    }

    public IEnumerable<DisponibilidadeHorarioDTO> GetAll()
    {
        try
        {
            return _context.DisponibilidadeHorarios
                .AsNoTracking()
                .Select(c => new DisponibilidadeHorarioDTO
                {
                    Id = c.Id,
                    Dia = c.Dia,
                    HorarioInicio = c.HorarioInicio,
                    HorarioFim = c.HorarioFim,
                    IdProfessor = c.IdProfessor
                })
                .ToList();

            
        }
        catch (Exception ex)
        {
           
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
            return new DisponibilidadeHorarioDTO
            {
                Id = disponibilidadeHorario.Id,
                Dia = disponibilidadeHorario.Dia,
                HorarioInicio = disponibilidadeHorario.HorarioInicio,
                HorarioFim = disponibilidadeHorario.HorarioFim,
                IdProfessor = disponibilidadeHorario.IdProfessor
            };


        }
        catch (Exception ex)
        {
            
            throw new ServiceException($"Erro ao buscar ID {id}", ex);
        }
    }




    public int Create(DisponibilidadeHorarioDTO disponibilidadeHorarioDto)
    {
        try
        {

            if (disponibilidadeHorarioDto.HorarioInicio == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim <= disponibilidadeHorarioDto.HorarioInicio)
                throw new ServiceException("O horário final deve ser posterior ao horário inicial");

            if (disponibilidadeHorarioDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");



            var disponibilidadeHorario = new DisponibilidadeHorario
            {
                Dia = DateTime.Today,
                HorarioInicio = disponibilidadeHorarioDto.HorarioInicio,
                HorarioFim = disponibilidadeHorarioDto.HorarioFim,
                IdProfessor = disponibilidadeHorarioDto.IdProfessor
            };

            
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
            
            throw new ServiceException("Erro ao criar horário", ex);
        }
    }


    public bool Update(DisponibilidadeHorarioDTO disponibilidadeHorarioDto)
    {
        try
        {

            if (disponibilidadeHorarioDto.HorarioInicio == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim == TimeSpan.MinValue)
                throw new ServiceException("campo obrigatório");

            if (disponibilidadeHorarioDto.HorarioFim <= disponibilidadeHorarioDto.HorarioInicio)
                throw new ServiceException("O horário final deve ser posterior ao horário inicial");

            if (disponibilidadeHorarioDto.IdProfessor <= 0)
                throw new ServiceException("campo obrigatório");

            var disponibilidadeHorario = _context.DisponibilidadeHorarios.Find(disponibilidadeHorarioDto.Id);
            if (disponibilidadeHorario == null)
                return false;
            disponibilidadeHorario.HorarioInicio = disponibilidadeHorarioDto.HorarioInicio;
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
           
            throw new ServiceException($"Erro ao excluir ID {id}", ex);
        }
    }
}
