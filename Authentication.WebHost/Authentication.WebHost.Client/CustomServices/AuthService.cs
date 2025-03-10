﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Authentication.Shared.Model;
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

    // REGISTER / ADD (BOTH SAME)
    public async Task<bool> Register(UserViewModel user)
    {
        var response = await _http.PostAsJsonAsync("/register", user);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> Add(UserViewModel user)
    {
        var response = await _http.PostAsJsonAsync("/add", user);
        return response.IsSuccessStatusCode;
    }


    public async Task<bool> Update(UserViewModel user)
    {
        var response = await _http.PutAsJsonAsync("/update", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserViewModel> Login(UserViewModel user)
    {
        var response = await _http.PostAsJsonAsync("/login", user);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserViewModel>();
            await _js.InvokeVoidAsync("localStorage.setItem", "token", result.token); // Store JWT in localStorage
            return result;
        }
        return null;
    }

    public async Task<UserViewModel> GetByEmail(string email)
    {
        var result = await _http.GetFromJsonAsync<UserViewModel>($"/GetByEmail/{email}");
        return result;
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "token"); // Remove JWT from localStorage
    }

    public async Task<string> GetToken()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", "token") ?? "";
    }

    public async Task<List<UserViewModel>> GeAllUser()
    {
        var token = await GetToken();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await _http.GetFromJsonAsync<List<UserViewModel>>("GetAllUser");
        return result;
    }
}

public class JwtResponse
{
    public string Token { get; set; }
}
