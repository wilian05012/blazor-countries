using System;
using System.Net.Http.Json;

namespace countries.ui.Services;

public class RestCountriesApiClient {
    private readonly HttpClient _httpClient;
    public RestCountriesApiClient(string baseUrl, HttpClient httpClient) :this(httpClient) {
        _httpClient.BaseAddress = new Uri(baseUrl);
    }
    public RestCountriesApiClient(HttpClient httpClient)  {
        _httpClient = httpClient;
    }

    #region Get All countries
    private const string ENDPOINT_ALL = "all?fields=cca3,name,population,flags,region,subregion,capital,languages,currencies,borders";
    public IAsyncEnumerable<Domain.Country?> GetAllCountriesAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsAsyncEnumerable<Domain.Country>(ENDPOINT_ALL, cancellationToken);
    #endregion

    #region Search countries by name
    private const string ENDPOINT_NAME_SEARCH = "name/";
    public IAsyncEnumerable<Domain.Country?> SearchByNameAsync(string countryName, CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsAsyncEnumerable<Domain.Country?>($"{ENDPOINT_NAME_SEARCH}{countryName}", cancellationToken);
    #endregion

    #region Filter countries by region
    private const string ENDPOINT_REGION_FILTER = "region/";
    public IAsyncEnumerable<Domain.Country?> FilterByRegionAsync(string regionName, CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsAsyncEnumerable<Domain.Country?>($"{ENDPOINT_REGION_FILTER}{regionName}", cancellationToken);
    #endregion

    #region Get country by code
    private const string ENDPOINT_BY_CODE = "alpha/";
    public async Task<Domain.Country?> GetCountryByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        (await _httpClient.GetFromJsonAsync<Domain.Country[]?>($"{ENDPOINT_BY_CODE}{code}", cancellationToken))?.FirstOrDefault();
    #endregion
}
