using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Components;

namespace countries.ui.Pages;

public partial class Home {
    [Inject]
    public Services.RestCountriesApiClient? ApiClient { get; set; }

    [Inject]
    private NavigationManager? _navMgr { get; set; }

    private readonly IList<Domain.Country> _countries = new List<Domain.Country>();
    private string _searchArg = string.Empty;

    private string _filterRegion = string.Empty;

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        if (ApiClient is null) throw new ApplicationException($"API client service was not instatiated!");

        await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync(CancellationToken cancellationToken = default) {
        _countries.Clear();
        await foreach (var country in ApiClient!.GetAllCountriesAsync(cancellationToken)) {
            if (cancellationToken.IsCancellationRequested) break;
            if (country is not null) _countries.Add(country);
        }
    }

    private async Task SearchByNameAsync(CancellationToken cancellationToken = default) {
        _countries.Clear();
        if (_filterAndSearchEmpty) {
            await LoadCountriesAsync(cancellationToken);
            return;
        }
        if (string.IsNullOrWhiteSpace(_searchArg)) {
            await FilterByRegionAsync(cancellationToken);
            return;
        }

        await foreach (var country in ApiClient!.SearchByNameAsync(_searchArg, cancellationToken)) {
            if (cancellationToken.IsCancellationRequested) break;

            if (country is null) continue;

            if (string.IsNullOrWhiteSpace(_filterRegion) || country.Region.Equals(_filterRegion) == true)
                _countries.Add(country);
        }
    }

    private async Task FilterByRegionAsync(CancellationToken cancellationToken = default) {
        _countries.Clear();
        if (_filterAndSearchEmpty) {
            await LoadCountriesAsync(cancellationToken);
            return;
        }

        if (string.IsNullOrWhiteSpace(_filterRegion)) {
            await SearchByNameAsync(cancellationToken);
            return;
        }

        await foreach (var country in ApiClient!.FilterByRegionAsync(_filterRegion, cancellationToken)) {
            if (cancellationToken.IsCancellationRequested) break;
            if (country is null) continue;

            if (string.IsNullOrWhiteSpace(_searchArg) || country.Name.Matches(_searchArg))
                _countries.Add(country);
        }
    }

    private bool _filterAndSearchEmpty =>
        string.IsNullOrWhiteSpace(_searchArg) &&
        string.IsNullOrWhiteSpace(_filterRegion);

    private void NavigateToCountry(string code) {
        _navMgr?.NavigateTo($"countries/{code.ToLowerInvariant()}");
    }
}
