using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mustaxe.Application.Services.CEP.Queries;
using System.Threading.Tasks;

namespace Mustaxe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CEPController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CEPController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpGet("consultar/{cep}")]
        public async Task<IActionResult> ConsultarCEP([FromRoute] string cep) 
        {
            var query = new ConsultarCEPQuery(cep);

            var result = await _mediator.Send(query);

            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}
