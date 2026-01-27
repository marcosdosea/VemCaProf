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
    private readonly ILogger<CidadeService> _logger;

    public CidadeService(
        VemCaProfContext context,
        IMapper mapper,
        ILogger<CidadeService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    /// <summary>
    /// Retorna todas as cidades cadastradas
    /// </summary>
    /// <returns>lista de cidades</returns>
    public IEnumerable<CidadeDTO> GetAll()
    {
        try
        {
            var cidades = _context.Cidades
                .AsNoTracking()
                .ToList();

            return _mapper.Map<IEnumerable<CidadeDTO>>(cidades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todas as cidades");
            throw new ServiceException("Erro ao buscar cidades", ex);
        }
    }
    /// <summary>
    /// Busca uma cidade pelo identificador
    /// </summary>
    /// <param name="id">id da cidade</param>
    /// <returns>dados da cidade</returns>
    public CidadeDTO? Get(int id)
    {
        try
        {
            var cidade = _context.Cidades
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            if (cidade == null)
                return null;

            return _mapper.Map<CidadeDTO>(cidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cidade com ID {Id}", id);
            throw new ServiceException($"Erro ao buscar cidade com ID {id}", ex);
        }
    }

    /// <summary>
    /// Busca uma cidade pelo nome e estado
    /// </summary>
    /// <param name="nome">nome da cidade</param>
    /// <param name="estado">sigla do estado</param>
    /// <returns>dados da cidade</returns>
    public CidadeDTO? GetByNomeEstado(string nome, string estado)
    {
        try
        {
            var cidade = _context.Cidades
                .AsNoTracking()
                .FirstOrDefault(c =>
                    c.Nome.ToLower() == nome.ToLower() &&
                    c.Estado.ToUpper() == estado.ToUpper());

            if (cidade == null)
                return null;

            return _mapper.Map<CidadeDTO>(cidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cidade {Nome}/{Estado}", nome, estado);
            throw new ServiceException($"Erro ao buscar cidade {nome}/{estado}", ex);
        }
    }

    /// <summary>
    /// Cadastra uma nova cidade na base de dados
    /// </summary>
    /// <param name="cidadeDto">dados da cidade</param>
    /// <returns>id da cidade</returns>
    public int Create(CidadeDTO cidadeDto)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(cidadeDto.Nome))
                throw new ServiceException("Nome da cidade é obrigatório");

            if (string.IsNullOrWhiteSpace(cidadeDto.Estado) || cidadeDto.Estado.Length != 2)
                throw new ServiceException("Estado deve ter 2 caracteres");


            var existing = GetByNomeEstado(cidadeDto.Nome, cidadeDto.Estado);
            if (existing != null)
                throw new ServiceException($"Cidade {cidadeDto.Nome}/{cidadeDto.Estado} já cadastrada");

            var cidade = _mapper.Map<Cidade>(cidadeDto);
            cidade.Estado = cidade.Estado.Trim().ToUpper();

            _context.Cidades.Add(cidade);
            _context.SaveChanges();

            return cidade.Id;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cidade");
            throw new ServiceException("Erro ao criar cidade", ex);
        }
    }

    /// <summary>
    /// Atualiza os dados de uma cidade existente
    /// </summary>
    /// <param name="cidadeDto">dados atualizados da cidade</param>
    /// <returns>true se atualizado com sucesso</returns>
    public bool Update(CidadeDTO cidadeDto)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(cidadeDto.Nome))
                throw new ServiceException("Nome da cidade é obrigatório");

            if (string.IsNullOrWhiteSpace(cidadeDto.Estado) || cidadeDto.Estado.Length != 2)
                throw new ServiceException("Estado deve ter 2 caracteres");

            var cidade = _context.Cidades.Find(cidadeDto.Id);
            if (cidade == null)
                return false;


            var existing = GetByNomeEstado(cidadeDto.Nome, cidadeDto.Estado);
            if (existing != null && existing.Id != cidadeDto.Id)
                throw new ServiceException($"Cidade {cidadeDto.Nome}/{cidadeDto.Estado} já cadastrada");


            cidade.Nome = cidadeDto.Nome.Trim();
            cidade.Estado = cidadeDto.Estado.Trim().ToUpper();

            _context.Cidades.Update(cidade);
            _context.SaveChanges();

            return true;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cidade ID {Id}", cidadeDto.Id);
            throw new ServiceException($"Erro ao atualizar cidade ID {cidadeDto.Id}", ex);
        }
    }

    /// <summary>
    /// Remove uma cidade da base de dados
    /// </summary>
    /// <param name="id">id da cidade</param>
    /// <returns>true se removida com sucesso</returns>
    public bool Delete(int id)
    {
        try
        {
            var cidade = _context.Cidades.Find(id);
            if (cidade == null)
                return false;

            _context.Cidades.Remove(cidade);
            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir cidade ID {Id}", id);
            throw new ServiceException($"Erro ao excluir cidade ID {id}", ex);
        }
    }
}
