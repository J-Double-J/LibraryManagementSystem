using Application.CQRS.CheckoutCQRS;
using Domain.Abstract;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : Controller
    {
        ISender _sender;

        public CheckoutController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutBook([FromBody]CheckoutBookCommand command)
        {
            Result result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                return Ok("Book successfully checked out!");
            }

            if (result is DomainValidationResult validationResult)
            {
                return BadRequest(validationResult.ValidationErrors);
            }

            return StatusCode(500, result.Error);
        }
    }
}
