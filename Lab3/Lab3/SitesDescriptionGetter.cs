using System.Net.Http.Json;
using Lab3.Reports;

namespace Lab3;

public class SitesDescriptionGetter
{
    public async Task<InterestingSitesData> GetDescriptions(List<Site> sitesData)
    {
        string apiKey = "5ae2e3f221c38a28845f05b6c804ccb3398f7bea7239a98a63af8f8c";
        var queiries = new List<Task<HttpResponseMessage>>();
        HttpClient client = new HttpClient();
        foreach (var site in sitesData)
        {
            string query = $"https://api.opentripmap.com/0.1/en/places/xid/{site.Xid}?apikey={apiKey}";
            queiries.Add(client.GetAsync(query));
        }
        var descriptions = await Task.WhenAll(queiries);
        var interestingSitesData = new InterestingSitesData(new List<SiteData>());
        for (int i = 0; i < descriptions.Length; i++)
        {
            var answer = await descriptions[i].Content.ReadFromJsonAsync<Description>();
            interestingSitesData.Sites.Add(new SiteData(sitesData[i], answer));
        }
        return interestingSitesData;
    }
}