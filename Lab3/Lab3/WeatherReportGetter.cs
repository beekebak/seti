using System.Net.Http.Json;
using Lab3.Reports;

namespace Lab3;

public class WeatherReportGetter
{
    public async Task<String> GetWeatherReport(Hit location)
    {
        string apiKey = "fec30491de2253da46b959a383e35e96";
        string query = $"https://api.openweathermap.org/data/2.5/weather?lat={location.point.lat}&lon=" +
                       $"{location.point.lng}&appid={apiKey}&units=metric";
        HttpClient client = new HttpClient();
        using HttpResponseMessage request = await client.GetAsync(query);
        var answer = await request.Content.ReadFromJsonAsync<WeatherData>();
        return $"""
                weather: {answer?.Weather[0].Name}
                description: {answer?.Weather[0].Description}
                temperature: {answer?.Chunk.Temperature}
                humidity: {answer?.Chunk.Humidity}
                pressure: {answer?.Chunk.Pressure} 
                """;
    }
}