using System.Net;
using System.Text.Json;
using AzureFunctionSample1.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionSample1.Functions;

public class WeatherFunction
{
    private readonly ILogger<WeatherFunction> _logger;

    public WeatherFunction(ILogger<WeatherFunction> logger)
    {
        _logger = logger;
    }

    [Function("WeatherFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] 
        HttpRequestData request)
    {
        _logger.LogInformation("Checking the weather...");
        
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (weatherRequest.Locations.Contains(1))
        {
            var weatherResponse = request.CreateResponse(HttpStatusCode.OK);
            await weatherResponse.WriteAsJsonAsync(new WeatherResponse()
            {
                Id = 72,
                Temperature = 20.2,
                Location = "London",
                Date = DateTime.Now,
                Summary = "Sunny"
            });
            return weatherResponse;
        }
        
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync("no weather data found");
        return response;
    }
}