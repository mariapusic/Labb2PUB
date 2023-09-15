using Labb2PUB.Server.Data;
using Labb2PUB.Server.Data.Models;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Countries in the world", Version = "v1" }));
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton(_ => new CountryContext(config.GetConnectionString("CosmosDB"), config["ContainerName"], config["DatabaseName"]));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
//app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("countries", async(CountryContext context) =>
{
    List<CountryModel> countries = await context.GetAllCountries();
    return Results.Ok(countries);
});

app.MapGet("country/{Id}", async(CountryContext context, string Id) =>
{
    CountryModel country = await context.GetCountry(Id);
    return Results.Ok(country);
});
app.MapPost("country", async (CountryContext context, CountryModel Cmodel) =>
{
    await context.AddCountry(Cmodel);
    return Results.Ok();
});
app.Run();
