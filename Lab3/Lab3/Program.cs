using Lab3;

async Task GetInformation(string siteRequest)
{
   LocationsGetter locationsGetter = new LocationsGetter();
   var location = await locationsGetter.GetLocations(siteRequest);
   WeatherReportGetter weatherReportGetter = new WeatherReportGetter();
   var weatherReport = weatherReportGetter.GetWeatherReport(location);
   InterestingSitesGetter interestingSitesGetter = new InterestingSitesGetter();
   var interestingSites = await interestingSitesGetter.GetInterestingSites(location);
   SitesDescriptionGetter sitesDescriptionGetter = new SitesDescriptionGetter();
   var sitesDescriptions = sitesDescriptionGetter.GetDescriptions(interestingSites);
   await Task.WhenAll(weatherReport, sitesDescriptions);
   Console.WriteLine($"Weather Report:{weatherReport.Result}");
   
   foreach (var sitesDescription in sitesDescriptions.Result.Sites)
   {
       Console.WriteLine($"{sitesDescription.Site.Name}: {sitesDescription.Description.Kinds}," +
                         $" {sitesDescription.Description.Info}");
   }
}


while (true)
{
    string? input = Console.ReadLine();
    if (input == "exit")
    {
        return 0;
    }
    await GetInformation(input);
}
