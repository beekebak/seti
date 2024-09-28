using System.Text.Json.Serialization;

namespace Lab3.Reports;

public record WeatherData(List<Weather> Weather, [property: JsonPropertyName("main")]MainDataChunk Chunk);

public record Weather([property: JsonPropertyName("main")]string Name, string Description);

public record MainDataChunk([property: JsonPropertyName("temp")]double Temperature, double Humidity, double Pressure);