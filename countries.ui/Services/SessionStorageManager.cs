using System;
using System.Text.Json;

using Microsoft.JSInterop;

namespace countries.ui.Services;

public class SessionStorageManager {
    private readonly IJSRuntime _jsRuntime;

    public SessionStorageManager(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
    }

    public async Task ClearAsync() =>
        await _jsRuntime.InvokeVoidAsync("window.sessionStorage.clear");
    public async Task SaveKeyedItemAsync<T>(string key, T value) {
        string stringifiedValue = JsonSerializer.Serialize(value);

        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, stringifiedValue);
    }

    public async Task RemoveKeyedItemAsync(string key) =>
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);

    public async Task<T?> GetKeyedItemAsync<T>(string key) {
        T? result = default;
        var stringifiedValue = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);

        if (!string.IsNullOrWhiteSpace(stringifiedValue))
            result = JsonSerializer.Deserialize<T>(stringifiedValue);

        return result;
    }
}
