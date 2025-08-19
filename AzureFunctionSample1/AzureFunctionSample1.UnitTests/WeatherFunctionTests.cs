using System.Net;
using System.Text.Json;
using Azure.Core.Serialization;
using AzureFunctionSample1.Functions;
using AzureFunctionSample1.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace AzureFunctionSample1.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Run_WhenProvidedWithAValidLocationAndDate_ReturnsWeatherData()
    {
        var body = new WeatherRequest { Date = new DateOnly(2025, 6, 15), Locations = [1] };
        var request = MockHttpRequestData(body);
        var function = new WeatherFunction(NullLogger<WeatherFunction>.Instance);
        
        var response = await function.Run(request);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public async Task Run_WhenProvidedWithAInvalidLocation_ReturnsOkWithNoDataMessage()
    {
        var body = new WeatherRequest { Date = new DateOnly(2025, 6, 15), Locations = [5] };
        var request = MockHttpRequestData(body);
        var function = new WeatherFunction(NullLogger<WeatherFunction>.Instance);
        
        var response = await function.Run(request);
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await new StreamReader(response.Body).ReadToEndAsync(), Is.EqualTo("no weather data found"));
    }

    //https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-unit-testing-dotnet-isolated#unit-testing-trigger-functions
    private HttpRequestData MockHttpRequestData(object requestBody, string httpMethod = "GET", HttpHeadersCollection? headers = null)
    {
        //
        // ObjectSerializer
        //
        var mockObjectSerializer = new Mock<ObjectSerializer>();
        mockObjectSerializer
            .Setup(s => s.SerializeAsync(It.IsAny<Stream>(), It.IsAny<object?>(), It.IsAny<Type>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, object?, Type, CancellationToken>(async (stream, value, type, token) => { await JsonSerializer.SerializeAsync(stream, value, type, cancellationToken: token); });

        var workerOptions = new WorkerOptions { Serializer = mockObjectSerializer.Object };
        var mockOptions = new Mock<IOptions<WorkerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(workerOptions);
        //
        // IServiceProvider
        //
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IOptions<WorkerOptions>)))
            .Returns(mockOptions.Object);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(ObjectSerializer)))
            .Returns(mockObjectSerializer.Object);
        //
        // FunctionContext
        //
        var mockFunctionContext = new Mock<FunctionContext>();
        mockFunctionContext.SetupGet(c => c.InstanceServices).Returns(mockServiceProvider.Object);
        //
        // HttpRequestData
        //
        var mockHttpRequestData = new Mock<HttpRequestData>(mockFunctionContext.Object);
        mockHttpRequestData.SetupGet(x => x.Method).Returns(httpMethod);
        mockHttpRequestData.SetupGet(r => r.Url).Returns(new Uri("https://localhost:7075/orchestrators/HelloCities"));
        //
        // Headers
        //
        headers ??= new HttpHeadersCollection();
        mockHttpRequestData.SetupGet(r => r.Headers).Returns(headers);
        //
        // Body
        //
        Stream bodyStream;
        if (requestBody != null)
        {
            var bodyJson = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(bodyJson);
            bodyStream = new MemoryStream(bodyBytes);
            mockHttpRequestData.SetupGet(r => r.Body).Returns(bodyStream);
        }

        //
        // HttpResponseData
        //
        var mockHttpResponseData = new Mock<HttpResponseData>(mockFunctionContext.Object) { DefaultValue = DefaultValue.Mock };
        mockHttpResponseData.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
        mockHttpResponseData.SetupProperty(r => r.Body, new MemoryStream());
        mockHttpRequestData.Setup(r => r.CreateResponse())
            .Returns(mockHttpResponseData.Object);

        return mockHttpRequestData.Object;
    }
}