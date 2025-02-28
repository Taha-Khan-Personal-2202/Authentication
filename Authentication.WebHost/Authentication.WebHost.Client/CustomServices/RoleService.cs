using Authentication.Shared.Model;
using Microsoft.JSInterop;
using System.Net.Http.Json;

public class RoleService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public RoleService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<bool> Add(string roleName)
    {
        var response = await _http.PostAsJsonAsync("/Add", roleName);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(string id, string roleName)
    {
        var response = await _http.PutAsJsonAsync($"/Update/{id}", roleName);
        return response.IsSuccessStatusCode;
    }

    public async Task<RoleViewModel> GetRoleById(string id)
    {
        return await _http.GetFromJsonAsync<RoleViewModel>($"/GetRoleById/{id}");
    }

    public async Task<List<RoleViewModel>> GetAllRoles()
    {
        return await _http.GetFromJsonAsync<List<RoleViewModel>>("/GetAllRoles");
    }

    public async Task<bool> DeleteRole(string id)
    {
        var response = await _http.DeleteAsync($"/Delete/{id}");
        return response.IsSuccessStatusCode;
    }
}
