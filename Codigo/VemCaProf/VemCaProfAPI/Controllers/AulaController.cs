using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using Core;
using Core.DTO;
using VemCaProfAPI.Models;
using Service;



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

        [HttpGet("horarios-disponiveis")]
        public ActionResult HorariosDisponiveis([FromQuery] int professorId, [FromQuery] DateTime dataAula, [FromQuery] int? aulaId = null)
        {
            var horarios = _aulaService.GetHorariosDisponiveis(professorId, dataAula, aulaId)
                .Select(h => new
                {
                    id = h.Id,
                    texto = $"{h.HorarioInicio:hh\\:mm} às {h.HorarioFim:hh\\:mm}"
                });

            return Ok(horarios);
        }

        // POST api/<AulaController>
        [HttpPost]
        public ActionResult Post([FromBody] AulaModel aulaModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var aula = _mapper.Map<AulaDTO>(aulaModel);
                var id = _aulaService.Create(aula);
                return CreatedAtAction(nameof(Get), new { id }, null);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<AulaController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] AulaModel aulaModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var aula = _mapper.Map<AulaDTO>(aulaModel);
                aula.Id = id;
                return _aulaService.Update(aula) ? Ok() : NotFound();
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
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


