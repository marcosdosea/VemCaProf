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
    public class PenalidadeController : ControllerBase
    {
        private readonly IPenalidadeService _penalidadeService;
        private readonly IMapper _mapper;


        public PenalidadeController(IPenalidadeService penalidadeService, IMapper mapper)
        {
            _penalidadeService = penalidadeService;
            _mapper = mapper;
        }

        // GET: api/<PenalidadeController>
        [HttpGet]
        public ActionResult Get()
        {
            var listaPenalidades = _penalidadeService.GetAll();
            if (listaPenalidades == null)
                return NotFound();

            return Ok(listaPenalidades);
        }


        // GET api/<PenalidadeController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var penalidade = _penalidadeService.Get(id);
            if (penalidade == null)
                return NotFound();

            return Ok(penalidade);
        }

        // POST api/<PenalidadeController>
        [HttpPost]
        public IActionResult Post([FromBody] PenalidadeDTO penalidadeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos");
            try
            {
                var id = _penalidadeService.Create(penalidadeDTO);
                var criado = _penalidadeService.Get(id);
                return CreatedAtAction(nameof(Get), new { id }, criado);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }


        }

        // PUT api/<PenalidadeController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] PenalidadeDTO penalidadeDTO)
        {
            
            penalidadeDTO.Id = id;
            var penalidade = _mapper.Map<PenalidadeDTO>(penalidadeDTO);
            if (penalidade == null)
                return NotFound();

            _penalidadeService.Edit(penalidade);
            return Ok();
        }

        // DELETE api/<PenalidadeController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            PenalidadeDTO ? penalidade = _penalidadeService.Get(id);
            if (penalidade == null)
                return NotFound();

            _penalidadeService.Delete(id);
            return Ok();



        }
    }
}
