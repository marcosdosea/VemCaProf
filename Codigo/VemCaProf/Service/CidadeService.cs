using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class CidadeService : ICidadeService
{
    private readonly VemCaProfContext _context;

    public CidadeService(VemCaProfContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todas as cidades cadastradas
    /// </summary>
    /// <returns>lista de cidades</returns>
    public IEnumerable<CidadeDTO> GetAll()
    {
        try
        {
            return _context.Cidades
                .AsNoTracking()
                .Select(c => new CidadeDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Estado = c.Estado
                })
                .ToList();
        }
        catch (Exception ex)
        {
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

            return new CidadeDTO
            {
                Id = cidade.Id,
                Nome = cidade.Nome,
                Estado = cidade.Estado
            };
        }
        catch (Exception ex)
        {
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

            return new CidadeDTO
            {
                Id = cidade.Id,
                Nome = cidade.Nome,
                Estado = cidade.Estado
            };
        }
        catch (Exception ex)
        {
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
            
            if (cidadeDto == null)
                throw new ServiceException("Dados da cidade não podem ser nulos");

            
            var nome = cidadeDto.Nome?.Trim() ?? "";
            var estado = cidadeDto.Estado?.Trim().ToUpper() ?? "";

            if (string.IsNullOrWhiteSpace(nome))
                throw new ServiceException("Nome da cidade é obrigatório");

            if (string.IsNullOrWhiteSpace(estado) || estado.Length != 2)
                throw new ServiceException("Estado deve ter 2 caracteres");

            var existing = GetByNomeEstado(nome, estado);
            if (existing != null)
                throw new ServiceException($"Cidade {nome}/{estado} já cadastrada");

            var cidade = new Cidade
            {
                Nome = nome,
                Estado = estado
            };

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
            
            if (cidadeDto == null)
                throw new ServiceException("Dados da cidade não podem ser nulos");

            
            var nome = cidadeDto.Nome?.Trim() ?? "";
            var estado = cidadeDto.Estado?.Trim().ToUpper() ?? "";

            if (string.IsNullOrWhiteSpace(nome))
                throw new ServiceException("Nome da cidade é obrigatório");

            if (string.IsNullOrWhiteSpace(estado) || estado.Length != 2)
                throw new ServiceException("Estado deve ter 2 caracteres");

            var cidade = _context.Cidades.Find(cidadeDto.Id);
            if (cidade == null)
                return false;

            var existing = GetByNomeEstado(nome, estado);
            if (existing != null && existing.Id != cidadeDto.Id)
                throw new ServiceException($"Cidade {nome}/{estado} já cadastrada");

            cidade.Nome = nome;
            cidade.Estado = estado;

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
            throw new ServiceException($"Erro ao excluir cidade ID {id}", ex);
        }
    }
}