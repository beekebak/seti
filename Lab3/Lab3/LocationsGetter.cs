using System.Net.Http.Json;

namespace Lab3;

public class LocationsGetter
{
    public async Task<Hit> GetLocations(string site)
    {
        HttpClient client = new();
        string apiKey = "403eef83-f97e-4b1d-bf85-354f65f9376d";
        string url = $"https://graphhopper.com/api/1/geocode?q={site}&locale=en" +
                     $"&provider=default&key={apiKey}";
        using HttpResponseMessage request = await client.GetAsync(url);
        var answer = await request.Content.ReadFromJsonAsync<GeoData>();
        Console.WriteLine("Choose one of the following locations:");
        for (int i = 0; i < answer?.hits.Count; i++)
        {
            Console.WriteLine($"{i} name: {answer.hits[i].name}, lat: {answer.hits[i].point.lat}, lon: " +
                              $"{answer.hits[i].point.lng}");
        }
        int idx = Int32.Parse(Console.ReadLine());
        return answer?.hits[idx] ?? throw new Exception();
    }
}