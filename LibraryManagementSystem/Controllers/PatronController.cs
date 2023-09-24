using Application.CQRS.PatronCQRS.Commands.AddPatron;
using Application.CQRS.PatronCQRS.Commands.DeletePatron;
using Application.CQRS.PatronCQRS.Queries;
using Domain.Abstract;
using Domain.Entities.Patron;
using Infrastructure.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class PatronController : Controller
    {
        private readonly IMediator _mediator;

        public PatronController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPatron([FromBody] AddPatronCommand addPatronCommand)
        {
            Result<Patron> result = await _mediator.Send(addPatronCommand);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result is DomainValidationResult<Patron> validationResult && validationResult.IsFailure)
            {
                return BadRequest(validationResult.ValidationErrors);
            }

            return StatusCode(500, result.Error!);
        }

        [HttpGet("patrons")]
        public async Task<IActionResult> GetAllPatrons()
        {
            Result<IEnumerable<Patron>> result = await _mediator.Send(new GetAllPatronsQuery());

            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return StatusCode(500, result.Error!);
        }

        [HttpGet("{patronGuid}")]
        public async Task<IActionResult> GetPatronByID(Guid patronGuid)
        {
            Result<Patron> result = await _mediator.Send(new GetPatronByIDQuery(patronGuid));

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if(result.Error!.Code == InfrastructureErrors.PatronRepositoryErrors.PATRON_ID_NOT_FOUND)
            {
                return NotFound(result.Error);
            }

            return StatusCode(500, result.Error!);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePatronByID([FromQuery] Guid patronGuid)
        {
            Result result = await _mediator.Send(new DeletePatronCommand(patronGuid));

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            if(result.Error!.Code == InfrastructureErrors.PatronRepositoryErrors.PATRON_ID_NOT_FOUND)
            {
                return NotFound(result.Error);
            }

            return StatusCode(500, result.Error!);
        }
    }
}
