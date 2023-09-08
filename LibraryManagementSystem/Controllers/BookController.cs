using Application.CQRS.BookCQRS.Queries;
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
    }
}