using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandlerTest.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SampleErrorController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<SampleErrorController> _logger;

    public SampleErrorController(ILogger<SampleErrorController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    
    [HttpGet]
    public IActionResult ProblemDetails()
    {
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet]
    public IActionResult Error()
    {
       throw new ArgumentException("There was an unhandled exception problem");
    }
}
