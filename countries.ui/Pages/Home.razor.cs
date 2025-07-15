using System;

using Microsoft.AspNetCore.Components;

namespace countries.ui.Pages;

public partial class Home {
    [Inject]
    public Services.RestCountriesApiClient? ApiClient { get; set; }

    private readonly IList<Domain.Country> countries = new List<Domain.Country>();

    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        if (ApiClient is null) throw new ApplicationException($"API client service was not instatiated!");

    }
}
