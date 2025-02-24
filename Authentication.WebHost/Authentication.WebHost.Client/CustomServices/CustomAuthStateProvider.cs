using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;


public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "token");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("❌ No token found. Returning empty authentication state.");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Decode JWT if applicable (Optional)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "User"),
            new Claim(ClaimTypes.Role, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User")
        };

            // Add Permission Claims if available
            var permissionClaims = jwtToken.Claims.Where(c => c.Type == "Permission").ToList();
            if (permissionClaims.Any())
            {
                claims.AddRange(permissionClaims);
                foreach (var claim in permissionClaims)
                {
                    Console.WriteLine($"✅ Loaded Permission Claim: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("⚠️ No permission claims found in token.");
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            _currentUser = new ClaimsPrincipal(identity);
            Console.WriteLine("✅ User authenticated successfully!");

            return new AuthenticationState(_currentUser);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error parsing JWT: {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }


    public void NotifyUserAuthentication(string token, string userName, string role, IList<string>? permissions)
    {
        var identity = new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.Name, userName),
        new Claim(ClaimTypes.Role, role)
    }, "jwt");

        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                identity.AddClaim(new Claim("Permission", permission));
                Console.WriteLine($"✅ Added Permission Claim: {permission}");
            }
        }
        else
        {
            Console.WriteLine("❌ No permissions found for user!");
        }

        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }



    public void NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}