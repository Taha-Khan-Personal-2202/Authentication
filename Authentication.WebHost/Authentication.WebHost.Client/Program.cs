using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5215") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();

builder.Services.AddAuthorizationCore();

// Register the Custom Authentication Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Use it for Blazor Authentication

await builder.Build().RunAsync(); // Only one call to RunAsync()
