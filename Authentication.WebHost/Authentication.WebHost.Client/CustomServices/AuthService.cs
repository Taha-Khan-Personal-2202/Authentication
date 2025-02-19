using System.Net.Http.Headers;
using System.Net.Http.Json;
using Authentication.Shared.Models;
using Microsoft.JSInterop;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<bool> Register(User user)
    {
        var response = await _http.PostAsJsonAsync("/register", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> Login(User user)
    {
        var response = await _http.PostAsJsonAsync("/login", user);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            await _js.InvokeVoidAsync("localStorage.setItem", "token", result); // Store JWT in localStorage
            return result;
        }
        return string.Empty;
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "token"); // Remove JWT from localStorage
    }

    public async Task<string> GetToken()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", "token") ?? "";
    }

    public async Task<List<User>> GeAllUesr()
    {
        //_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken().Result);
        var result = await _http.GetFromJsonAsync<List<User>>("/GetAllUser");
        return result;
    }
}

public class JwtResponse
{
    public string Token { get; set; }
}
