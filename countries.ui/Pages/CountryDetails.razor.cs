using System;

using countries.ui.Domain;

using Microsoft.AspNetCore.Components;

namespace countries.ui.Pages;

public partial class CountryDetails {
    [Inject]
    private Services.RestCountriesApiClient? _apiClient { get; set; }

    [Inject]
    private NavigationManager? _navMgr { get; set; }

    [Parameter]
    public string Code { get; set; } = string.Empty;

    private Domain.Country? _country;
    private readonly IList<Domain.Country> _neighboringCountries = new List<Domain.Country>();
    private bool _isLoading = false;
    private string _errMsg = string.Empty;

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        _isLoading = true;
        try {
            _country = await _apiClient!.GetCountryByCodeAsync(Code);
            _neighboringCountries.Clear();
            if (_country?.Borders?.Any() == true) {
                foreach (var border in _country.Borders) {
                    var neighborCountry = await _apiClient!.GetCountryByCodeAsync(border);
                    if (neighborCountry is not null) {
                        _neighboringCountries.Add(neighborCountry);
                    }
                }

            }
        } catch(Exception e) {
            _errMsg = e.Message;
        } finally { 
            _isLoading = false;
        }
    }

    private void NavigateBackHome() {
        _navMgr?.NavigateTo("/");
    }

    private void NavigateToCountry(string countryCode) {
        _navMgr?.NavigateTo($"countries/{countryCode}");
    }
}
