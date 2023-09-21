using Application.CQRS.BookCQRS.Commands;
using Application.CQRS.BookCQRS.Commands.RemoveBook;
using Application.CQRS.BookCQRS.Queries;
using Domain.Abstract;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Errors;
using Application.CQRS.BookCQRS.Commands.UpdateBook;

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

            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result is DomainValidationResult<Book> validationResult && validationResult.IsFailure)
            {
                return BadRequest(validationResult.ValidationErrors);
            }

            return BadRequest(result.Error);
        }

        [HttpPost("remove")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookByID(Guid id)
        {
            Result<Book> result = await _mediator.Send(new GetBookByIDQuery(id));

            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error!.Code == InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND)
            {
                return BadRequest(result);
            }

            return StatusCode(500, result.Error);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookCommand command)
        {
            Result<Book> result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error!.Code == InfrastructureErrors.BookRepositoryErrors.BOOK_ID_NOT_FOUND)
            {
                return BadRequest(result);
            }

            if (result is DomainValidationResult<Book> validationResult)
            {
                return BadRequest(validationResult.ValidationErrors);
            }

            return StatusCode(500, result.Error);
        }
    }
}