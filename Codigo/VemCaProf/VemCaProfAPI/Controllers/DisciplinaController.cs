using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using VemCaProfWeb.Models;
using Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VemCaProfAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplinaController : ControllerBase
    {
        private readonly IDisciplinaService _disciplinaService;
        private readonly IMapper _mapper;

        public DisciplinaController(IDisciplinaService disciplinaService, IMapper mapper)
        {
            _disciplinaService = disciplinaService;
            _mapper = mapper;
        }

        // GET: api/<DisciplinaController>
        [HttpGet]
        public ActionResult Get() 
        {
            var listaDisciplinas = _disciplinaService.GetAll();
            if(listaDisciplinas == null)
                return NotFound();

            return Ok(listaDisciplinas);
        }

        // GET api/<DisciplinaController>/5
        [HttpGet("{id}")]
        public ActionResult Get(uint id)
        {
            Disciplina disciplina = _disciplinaService.Get(id);
            if (disciplina == null)
                return NotFound();
            return Ok(disciplina);
        }

        // POST api/<DisciplinaController>
        [HttpPost]
        public ActionResult Post([FromBody] DisciplinaModel disciplinaModel)
        {
            if (ModelState.IsValid)
                return BadRequest("Dados inválidos");

            var disciplina = _mapper.Map<Disciplina>(disciplinaModel);
            _disciplinaService.Create(disciplina);

            return Ok();
        }

        // PUT api/<DisciplinaController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
            if (ModelState.IsValid)
                return BadRequest("Dados inválidos");

            var disciplina = _mapper.Map<Disciplina>(value);
            if (disciplina == null)
                return NotFound();

            _disciplinaService.Edit(disciplina);

            return Ok();
        }

        // DELETE api/<DisciplinaController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(uint id)
        {
            Disciplina? disciplina = _disciplinaService.Get(id);
            if (disciplina == null)
                return NotFound();

            _disciplinaService.Delete(id);
            return Ok();
        }
    }
}
