using Application.CQRS.CheckoutCQRS;
using Domain.Abstract;
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
        public async Task<IActionResult> CheckoutBook([FromBody] CheckoutBookCommand command)
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

        [HttpPut]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookCommand command)
        {
            Result result = await _sender.Send(command);

            if(result.IsSuccess)
            {
                return Ok("Book successfully returned!");
            }

            if (result is DomainValidationResult validationResult)
            {
                return BadRequest(validationResult.ValidationErrors);
            }

            return StatusCode(500, result.Error);
        }

        [HttpPut("renew")]
        public async Task<IActionResult> RenewBook([FromBody] RenewCheckoutCommand command)
        {
            Result<bool> result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                if (result.Value == true)
                {
                    return Ok("Your book is renewed for another session");
                }

                return UnprocessableEntity("Unable to renew your book. Book has reached its maximum renewal limit.");
            }

            return StatusCode(500, result.Error);
        }
    }
}
