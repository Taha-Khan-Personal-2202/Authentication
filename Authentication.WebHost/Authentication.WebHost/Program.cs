using System.Net.Http.Headers;
using System.Text;
using Authentication.Shared.Model;
using Authentication.WebHost.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7134") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();


builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Use it for Blazor Authentication

// 1. Read JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSetting");
var secret = jwtSettings["Secret"];
var secretKey = Encoding.UTF8.GetBytes(secret);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true, // Token expiration check
            ClockSkew = TimeSpan.Zero // No delay in expiration
        };

    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// Create a scope before calling `Build()`
using var scope = builder.Services.BuildServiceProvider().CreateScope();
var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
var permissions = await roleService.GetAllRolePermissionsAsync();

// Move authorization configuration before `Build()`
builder.Services.AddAuthorization(options =>
{
    foreach (var role in permissions)
    {
        foreach (var permission in role.Value)
        {
            options.AddPolicy(permission, policy =>
            {
                policy.RequireClaim("Permission", permission);
            });
        }
    }
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Authentication.WebHost.Client._Imports).Assembly);

app.Run();
