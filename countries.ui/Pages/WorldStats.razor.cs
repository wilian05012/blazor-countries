using System;

using countries.ui.Domain;

using Microsoft.AspNetCore.Components;

namespace countries.ui.Pages;

public partial class WorldStats {
    [Inject]
    private Services.RestCountriesApiClient? _apiClient { get; set; }

    private World? _world;
    private bool _isLoading = false;
    protected override async Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        _isLoading = true;
        try {
            _world = await _apiClient!.GetWorldCountriesAsync();
        } finally {
            _isLoading = false;
        }
    }
}
