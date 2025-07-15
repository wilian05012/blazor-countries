using System;

namespace countries.ui.Domain;

public class Country {
    public class CountryName {
        public string Common { get; set; } = string.Empty;
        public string Official { get; set; } = string.Empty;
    }
    public CountryName Name { get; set; } = new();
    public int Population { get; set; }
    public string Region { get; set; } = string.Empty;
    public string[] Capital { get; set; } = Array.Empty<string>();
    public string SubRegion { get; set; } = string.Empty;
    public string TopLevelDomain { get; set; } = string.Empty;

    public string[] Borders { get; set; } = Array.Empty<string>();
    public class CountryFlags {
        public string Svg { get; set; } = string.Empty;
        public string Png { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
    }
    public CountryFlags Flag { get; set; } = new();
}
