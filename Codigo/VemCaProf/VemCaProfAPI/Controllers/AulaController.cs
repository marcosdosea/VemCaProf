using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using VemCaProfWeb.Models;
using Core;
using Core.DTO;



namespace VemCaProfAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AulaController : ControllerBase
    {
        private readonly IAulaService _aulaService;
        private readonly IMapper _mapper;

        public AulaController(IAulaService aulaService, IMapper mapper)
        {
            _aulaService = aulaService;
            _mapper = mapper;
        }

        // GET: api/<AulaController>
        [HttpGet]
        public ActionResult Get()
        {
            var listaAulas = _aulaService.GetAll();
            if (listaAulas == null)
                return NotFound();

            return Ok(listaAulas);
        }

        // GET api/<AulaController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            AulaDTO aula = _aulaService.Get(id);
            if (aula == null)
                return NotFound();
            return Ok(aula);
        }

        // POST api/<AulaController>
        [HttpPost]
        public ActionResult Post([FromBody] AulaModel aulaModel)
        {
            if (ModelState.IsValid)
                return BadRequest("Dados inválidos");

            var aula = _mapper.Map<AulaDTO>(aulaModel);
            _aulaService.Create(aula);

            return Ok();
        }

        // PUT api/<AulaController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] AulaDTO aulaDTO)
        {

            aulaDTO.Id = id;
            var aula = _mapper.Map<AulaDTO>(aulaDTO);
            if (aula == null)
                return NotFound();

            _aulaService.Update(aula);

            return Ok();
        }

        // DELETE api/<AulaController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            AulaDTO? aula = _aulaService.Get(id);
            if (aula == null)
                return NotFound();

            _aulaService.Delete(id);
            return Ok();
        }
    }
}


