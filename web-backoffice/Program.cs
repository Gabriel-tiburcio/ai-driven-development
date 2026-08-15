using AllStay.Web.Backoffice.Components;
using AllStay.Web.Backoffice.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // TEMPORARY: shows the real exception in the circuit error banner instead of
        // "An unhandled error has occurred." Remove once the login issue is diagnosed —
        // this leaks stack traces to anyone whose circuit crashes.
        options.DetailedErrors = true;
    });

builder.Services.AddScoped<AuthState>();
builder.Services.AddTransient<AuthHeaderHandler>();
builder.Services.AddScoped<ApiClient>();

builder.Services.AddHttpClient("AllStayApi", client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://api.allstay.eupanda.com.br";
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<AuthHeaderHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
