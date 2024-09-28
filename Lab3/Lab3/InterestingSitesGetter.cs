using System.Globalization;
using System.Net.Http.Json;
using Lab3.Reports;

namespace Lab3;

public class InterestingSitesGetter
{
    public async Task<List<Site>> GetInterestingSites(Hit location)
    {
        string apiKey = "5ae2e3f221c38a28845f05b6c804ccb3398f7bea7239a98a63af8f8c";
        string query = $"https://api.opentripmap.com/0.1/en/places/radius?radius=20000&" +
                       $"lon={location.point.lng.ToString("F4", CultureInfo.InvariantCulture)}&" +
                       $"lat={location.point.lat.ToString("F4", CultureInfo.InvariantCulture)}&" +
                       $"kinds=interesting_places&format=json&limit=5&apikey={apiKey}";
        HttpClient client = new HttpClient();
        using HttpResponseMessage request = await client.GetAsync(query);
        var answer = await request.Content.ReadFromJsonAsync<List<Site>>();
        return answer;
    }
}