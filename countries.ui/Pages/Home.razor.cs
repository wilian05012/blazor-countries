using System;
using System.Diagnostics;
using System.Threading.Tasks;

using countries.ui.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace countries.ui.Pages;

public partial class Home {
    private const string STATUS_KEY = "status";

    [Inject]
    public Services.RestCountriesApiClient? ApiClient { get; set; }

    [Inject]
    private NavigationManager? _navMgr { get; set; }

    [Inject]
    private IJSRuntime? _jsRuntime { get; set; }

    [Inject]
    private SessionStorageManager? _storageManager { get; set; }


    private IJSObjectReference? _collocatedJsModule;
    private readonly IList<Domain.Country> _countries = new List<Domain.Country>();
    private string _searchArg = string.Empty;
    private string _filterRegion = string.Empty;
    private bool _isLoading = false;

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        _collocatedJsModule ??= await _jsRuntime!.InvokeAsync<IJSObjectReference>("import", "./Pages/Home.razor.js");

        await RetrieveStatusAsync();

        if (!string.IsNullOrWhiteSpace(_searchArg)) await SearchByNameAsync();
        else if (!string.IsNullOrWhiteSpace(_filterRegion)) await FilterByRegionAsync();
        else await LoadCountriesAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        await base.OnAfterRenderAsync(firstRender);

        if (!string.IsNullOrWhiteSpace(_status?.SelectedCountryCode)) {
            await BringCountryIntoViewAsync(_status.SelectedCountryCode);
            _status.SelectedCountryCode = null;
        }
    }

    private async Task LoadCountriesAsync(CancellationToken cancellationToken = default) {
        _countries.Clear();
        _isLoading = true;

        try {
            await foreach (var country in ApiClient!.GetAllCountriesAsync(cancellationToken)) {
                if (cancellationToken.IsCancellationRequested) break;
                if (country is not null) _countries.Add(country);
            }
        } finally {
            _isLoading = false;
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

        _isLoading = true;
        try {
            await foreach (var country in ApiClient!.SearchByNameAsync(_searchArg, cancellationToken)) {
                if (cancellationToken.IsCancellationRequested) break;

                if (country is null) continue;

                if (string.IsNullOrWhiteSpace(_filterRegion) || country.Region.Equals(_filterRegion) == true)
                    _countries.Add(country);
            }
        } finally {
            _isLoading = false;
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

        _isLoading = true;
        try {
            await foreach (var country in ApiClient!.FilterByRegionAsync(_filterRegion, cancellationToken)) {
                if (cancellationToken.IsCancellationRequested) break;
                if (country is null) continue;

                if (string.IsNullOrWhiteSpace(_searchArg) || country.Name.Matches(_searchArg))
                    _countries.Add(country);
            }
        } finally {
            _isLoading = false;
        }
    }

    private async Task BringCountryIntoViewAsync(string countryCode) {
        await _collocatedJsModule!.InvokeVoidAsync("bringInToView", countryCode.ToLowerInvariant());
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
