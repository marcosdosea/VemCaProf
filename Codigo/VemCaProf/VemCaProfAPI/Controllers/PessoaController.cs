using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using Core;
using VemCaProfAPI.Models; 

namespace VemCaProfAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public PessoaController(IPessoaService pessoaService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        // GET: api/pessoa
        [HttpGet]
        public ActionResult Get()
        {
            var listaPessoas = _pessoaService.GetAll();
            if (listaPessoas == null || !listaPessoas.Any())
                return NotFound("Nenhuma pessoa encontrada.");

            return Ok(listaPessoas);
        }

        // GET api/pessoa/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null)
                return NotFound($"Pessoa com ID {id} não encontrada.");

            return Ok(pessoa);
        }

        // POST api/pessoa
        [HttpPost]
        public ActionResult Post([FromBody] PessoaApiModel pessoaModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pessoa = _mapper.Map<Pessoa>(pessoaModel);
            var id = _pessoaService.Create(pessoa);

            return CreatedAtAction(nameof(Get), new { id }, pessoa);
        }

        // PUT api/pessoa/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] PessoaApiModel pessoaModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se a pessoa existe
            var pessoaExistente = _pessoaService.Get(id);
            if (pessoaExistente == null)
                return NotFound($"Pessoa com ID {id} não encontrada.");

            // Mapeia e garante que o ID seja mantido
            var pessoa = _mapper.Map<Pessoa>(pessoaModel);
            pessoa.Id = id;

            _pessoaService.Edit(pessoa);

            return NoContent(); // 204 – sucesso sem conteúdo
        }

        // DELETE api/pessoa/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa == null)
                return NotFound($"Pessoa com ID {id} não encontrada.");

            _pessoaService.Delete(id);
            return NoContent();
        }
    }
}