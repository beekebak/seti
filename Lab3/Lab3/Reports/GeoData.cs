using System.Text.Json.Serialization;

namespace Lab3;

public record GeoData(List<Hit> hits);
public record Hit(Point point, string name);
public record Point(double lat, double lng);