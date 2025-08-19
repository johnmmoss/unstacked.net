using System.Runtime.InteropServices.JavaScript;

namespace AzureFunctionSample1.Models;

public class WeatherRequest
{
    public DateOnly Date { get; set; }
    public int[] Locations { get; set; }
}