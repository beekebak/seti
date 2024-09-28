namespace Lab3.Reports;

public record InterestingSitesData(List<SiteData> Sites);

public record SiteData(Site Site, Description Description);

public record Site(string Name, string Xid);

public record Description(string Kinds, Info Info);

public record Info(string Descr);