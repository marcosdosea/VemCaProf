using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace VemCaProfAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CidadeController : ControllerBase
    {
        private readonly ICidadeService _cidadeService;

        public CidadeController(ICidadeService cidadeService)
        {
            _cidadeService = cidadeService;
        }

        // GET: api/Cidade
        [HttpGet]
        public IActionResult Get()
        {
            var lista = _cidadeService.GetAll();
            return Ok(lista);
        }

        // GET: api/Cidade/5
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var cidade = _cidadeService.Get(id);
            if (cidade == null) return NotFound();
            return Ok(cidade);
        }

        // EXTRA: GET: api/Cidade/search?nome=Aracaju&estado=SE
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string nome, [FromQuery] string estado)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(estado))
                return BadRequest("Informe nome e estado.");

            var cidade = _cidadeService.GetByNomeEstado(nome, estado);
            if (cidade == null) return NotFound();

            return Ok(cidade);
        }

        // POST: api/Cidade
        [HttpPost]
        public IActionResult Post([FromBody] CidadeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = _cidadeService.Create(dto);
                var criado = _cidadeService.Get(id);
                return CreatedAtAction(nameof(Get), new { id }, criado);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Cidade/5
        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] CidadeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest("Id da rota diferente do Id do corpo.");

            try
            {
                var ok = _cidadeService.Update(dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Cidade/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var ok = _cidadeService.Delete(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // EXTRA: GET api/Cidade/autocomplete?term=araca&limit=10
        [HttpGet("autocomplete")]
        public IActionResult Autocomplete([FromQuery] string term, [FromQuery] int limit = 10)
        {
            var lista = _cidadeService.AutocompleteByNome(term, limit);
            return Ok(lista);
        }
    }
}