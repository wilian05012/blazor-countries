using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using countries.ui;
using countries.ui.Services;

internal class Program {
    private static async Task Main(string[] args) {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddHttpClient("apiClient", httpClient => {
            httpClient.BaseAddress = new Uri(GetConfigValue(builder.Configuration, Constants.Settings.REST_COUNTRIES_API_BASE_URL));
        });
        builder.Services.AddScoped(sp => {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("apiClient");

            return new RestCountriesApiClient(GetConfigValue(builder.Configuration, Constants.Settings.REST_COUNTRIES_API_BASE_URL), httpClient);
        });

        builder.Services.AddSessionStorageManager();

        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }

    private static string GetConfigValue(IConfiguration configuration, string configKey) =>
        configuration[configKey] ?? throw new ApplicationException($"Missing configuration key: {configKey}");
}