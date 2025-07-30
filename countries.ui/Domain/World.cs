using System;

namespace countries.ui.Domain;

public class World {
    public class SubRegion {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Country> Countries { get; set; } = Array.Empty<Country>();
    }
    public class Region {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<SubRegion> SubRegions { get; set; } = Array.Empty<SubRegion>();
    }
    public static string[] RegionNames => new string[] {
        "Americas",
        "Europe",
        "Asia",
        "Africa",
        "Oceania",
        "Antarctic"
    };

    public IEnumerable<Region> Regions { get; set; } = RegionNames
        .Select(regionName => new Region() { Name = regionName })
        .ToArray();
}
