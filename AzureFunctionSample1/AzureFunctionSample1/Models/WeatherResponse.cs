namespace AzureFunctionSample1.Models;

public class WeatherResponse
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public string Summary { get; set; }
    public string Location { get; set; }
}