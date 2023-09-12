using Application.CQRS.BookCQRS.Commands;
using Application.CQRS.BookCQRS.Queries;
using Domain.Abstract;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}