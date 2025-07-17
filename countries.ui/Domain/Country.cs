using System;
using System.Text.Json.Serialization;

namespace countries.ui.Domain;

public class Country {
    [JsonPropertyName("cca3")]
    public string Code { get; set; } = string.Empty;
    public class CountryName {
        public string Common { get; set; } = string.Empty;
        public string Official { get; set; } = string.Empty;
        public Dictionary<string, CountryName> NativeName { get; set; } = new();

        public override string ToString() => Common;
    }
    public CountryName Name { get; set; } = new();
    public int Population { get; set; }
    public string Region { get; set; } = string.Empty;
    public string[] Capital { get; set; } = Array.Empty<string>();
    public string SubRegion { get; set; } = string.Empty;

    [JsonPropertyName("tld")]
    public string[] TopLevelDomain { get; set; } = Array.Empty<string>();

    public string[] Borders { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> Languages { get; set; } = new();
    public class Currency {
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
    public Dictionary<string, Currency> Currencies { get; set; } = new();

    public class CountryFlags {
        public string Svg { get; set; } = string.Empty;
        public string Png { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
    }

    [JsonPropertyName("flags")]
    public CountryFlags Flag { get; set; } = new();
}
