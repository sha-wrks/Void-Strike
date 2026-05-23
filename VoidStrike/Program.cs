using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VoidStrike;
using VoidStrike.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<InputManager>();
builder.Services.AddScoped<PhysicsEngine>();
builder.Services.AddScoped<GameEngine>();

await builder.Build().RunAsync();
