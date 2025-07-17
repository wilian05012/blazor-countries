using System;

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

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        _country = await _apiClient!.GetCountryByCodeAsync(Code);
    }

    private void NavigateBackHome() {
        _navMgr?.NavigateTo("/");
    }
}
