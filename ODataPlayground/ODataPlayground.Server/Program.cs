using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using NextjsStaticHosting.AspNetCore;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Step 1: Add Next.js hosting support
builder.Services.Configure<NextjsStaticHostingOptions>(builder.Configuration.GetSection("NextjsStaticHosting"));
builder.Services.AddNextjsStaticHosting();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});


builder.Services.AddControllers(options => options.InputFormatters.Add(new TextPlainFormatter()));


builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

builder.Services.AddScoped<UserService>();


var app = builder.Build();
app.UseRouting();

// Step 2: Register dynamic endpoints to serve the correct HTML files at the right request paths.
app.MapNextjsStaticHtmls();

// Step 3: Serve other required files (e.g. js, css files in the exported next.js app).
app.UseNextjsStaticHosting();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class TextPlainFormatter : InputFormatter
{
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        using (var reader = new System.IO.StreamReader(context.HttpContext.Request.Body))
        {
            return await InputFormatterResult.SuccessAsync(await reader.ReadToEndAsync());
        }
    }

    public override bool CanRead(InputFormatterContext context)
    {
        return true;
    }
}