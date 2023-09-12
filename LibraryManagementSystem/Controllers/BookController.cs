using Application.CQRS.BookCQRS.Commands;
using Application.CQRS.BookCQRS.Commands.RemoveBook;
using Application.CQRS.BookCQRS.Queries;
using Domain.Abstract;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Errors;

namespace Infrastructure
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : Controller
    {
        private IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Result<IEnumerable<Book>> result = await _mediator.Send(new GetAllBooksQuery());

            return result.IsSuccess ? Ok(result.Value) : StatusCode(500, result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] CreateBookCommand createBookCommand)
        {
            Result<Book> result = await _mediator.Send(createBookCommand);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveBook([FromQuery] Guid guid)
        {
            Result result = await _mediator.Send(new RemoveBookCommand(guid));

            if (result.IsSuccess)
            {
                return Ok("Successfully removed book from system.");
            }

            if (result.Error!.Code == InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND)
            {
                return BadRequest(result.Error!.Message);
            }

            return StatusCode(500, result.Error!.Message);
        }
    }
}