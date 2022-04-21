using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Prode2022Server.Data;
using Prode2022Server.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<DbService>();
builder.Services.AddSingleton<DataAdminServices>();

//swagger
builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddSwaggerGen();

//theme
builder.Services.AddMudServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}
else
{
    //swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



//Map controllers
app.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");
app.Run();
