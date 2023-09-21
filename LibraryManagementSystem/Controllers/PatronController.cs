using Application.CQRS.BookCQRS.Commands.AddPatron;
using Domain.Abstract;
using Domain.Entities;
using Domain.Entities.Patron;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class PatronController : Controller
    {
        private IMediator _mediator;

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
    }
}
