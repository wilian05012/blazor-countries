using System;

namespace countries.ui;

public static class Extensions {
    public static bool Matches(this Domain.Country.CountryName countryName, string pattern) =>
        countryName.Common.Contains(pattern, StringComparison.CurrentCultureIgnoreCase) ||
        countryName.Official.Contains(pattern, StringComparison.CurrentCultureIgnoreCase);
}
