using System;
using System.Diagnostics;
using System.Threading.Tasks;

using countries.ui.Services;

using Microsoft.AspNetCore.Components;

namespace countries.ui.Pages;

public partial class Home {
    private const string STATUS_KEY = "status";

    [Inject]
    public Services.RestCountriesApiClient? ApiClient { get; set; }

    [Inject]
    private NavigationManager? _navMgr { get; set; }

    [Inject]
    private SessionStorageManager? _storageManager { get; set; }
    private readonly IList<Domain.Country> _countries = new List<Domain.Country>();
    private string _searchArg = string.Empty;

    private string _filterRegion = string.Empty;

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        await RetrieveStatusAsync();

        if (!string.IsNullOrWhiteSpace(_searchArg)) await SearchByNameAsync();
        if (!string.IsNullOrWhiteSpace(_filterRegion)) await FilterByRegionAsync();
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

    private async Task NavigateToCountryAsync(string code) {
        await SaveStatusAsync(code);

        _navMgr?.NavigateTo($"countries/{code.ToLowerInvariant()}");
    }

    public class Status {
        public string RegionFilter { get; set; } = string.Empty;
        public string SearchArgument { get; set; } = string.Empty;
        public string? SelectedCountryCode { get; set; }
    }

    private Status? _status;
    private Task SaveStatusAsync(string code) {
        _status = new Status() {
            RegionFilter = _filterRegion,
            SearchArgument = _searchArg,
            SelectedCountryCode = code
        };

        return _storageManager!.SaveKeyedItemAsync(STATUS_KEY, _status);
    }

    private async Task RetrieveStatusAsync() {
        _status = await _storageManager!.GetKeyedItemAsync<Status>(STATUS_KEY);

        if (_status is not null) {
            _searchArg = _status.SearchArgument;
            _filterRegion = _status.RegionFilter;
        }
    }
}
