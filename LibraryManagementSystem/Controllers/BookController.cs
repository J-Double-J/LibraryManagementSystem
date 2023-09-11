using Application.CQRS.BookCQRS.Commands;
using Application.CQRS.BookCQRS.Queries;
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
            return Ok(await _mediator.Send(new GetAllBooksQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] CreateBookCommand createBookCommand)
        {
            return Ok(await _mediator.Send(createBookCommand));
        }
    }
}