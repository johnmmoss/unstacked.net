using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    
    private List<Book> _books = new List<Book>()
    {
        new Book(1, "The Hobbit", "J. R. R. Tolkien", "Harper Collins", new DateOnly(1937, 9, 21)),
        new Book(2, "Test Driven Development: By Example", "Kent Beck", "Addison-Wesley", new DateOnly(2002, 11, 8)),
        new Book(3, "The Unaccountability Machine: Why Big Systems Make Terrible Decisions", "Dan Davies", "Profile Books Ltd", new DateOnly(2025, 3, 13))
    };

    public BooksController(ILogger<BooksController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetBooks")]
    public ActionResult<IEnumerable<Book>> Get()
    {
        return Ok(_books);
    }
}
