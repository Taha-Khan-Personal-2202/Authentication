using Authentication.Shared.Model;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

public class RoleService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public RoleService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<bool> Add(RoleViewModel model)
    {
        var response = await _http.PostAsJsonAsync("/AddRoles", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(string id, RoleViewModel model)
    {
        model.ConcurrencyStamp = "";
        var response = await _http.PutAsJsonAsync($"/Update/{id}", model);
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

    public async Task<List<PermissionModel>> GetAllPermissions()
    {
        return await _http.GetFromJsonAsync<List<PermissionModel>>("/GetAllPermissions");
    }

    public async Task<Dictionary<string, List<string>>> GetAllRolePermissionsAsync()
    {
        return await _http.GetFromJsonAsync<Dictionary<string, List<string>>>("/GetRolePermissions")
               ?? new Dictionary<string, List<string>>();
    }

    public async Task<bool> DeleteRole(string id)
    {
        var response = await _http.DeleteAsync($"/Delete/{id}");
        return response.IsSuccessStatusCode;
    }
}
