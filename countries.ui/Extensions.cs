using System;

using countries.ui.Domain;

namespace countries.ui;

public static class Extensions {
    public static bool Matches(this Domain.Country.CountryName countryName, string pattern) =>
        countryName.Common.Contains(pattern, StringComparison.CurrentCultureIgnoreCase) ||
        countryName.Official.Contains(pattern, StringComparison.CurrentCultureIgnoreCase);

    public static string[] GetNativeNames(this Domain.Country.CountryName countryName) {
        return countryName.NativeName.Values
            .Select(name => name.Common)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToArray();
    }
}
