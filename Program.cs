using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Prode2022Server.Services;
using Prode2022Server.Security;
using MudBlazor.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blazored.LocalStorage;
using Prode2022Server.Models;
using Prode2022Server.Helpers;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DbService>();
builder.Services.AddSingleton<DataAdminServices>();
builder.Services.AddSingleton<SecurityServices>();
builder.Services.AddSingleton<SettingHelpers>();
builder.Services.AddScoped<TournamentsServices>();
builder.Services.AddScoped<UserTournamentsServices>();
builder.Services.AddScoped<AdminSiteServices>();
builder.Services.AddScoped<UserDataServices>();

//Add services as DI
builder.Services.AddSingleton<FixtureService>();
builder.Services.AddSingleton<ForecastService>();
//authentication

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddBlazoredLocalStorage();
//Authorization
builder.Services.AddScoped<IAuthorizationHandler, ProfileHandler>();

builder.Services.AddAuthorization(config =>
        {
            config.AddPolicy("ProfileIsAdmin", policy =>
                policy.Requirements.Add(new ProfileIsAdmin()));
        });
//Notifiers
builder.Services.AddScoped(typeof(IGenericListNotifier<>), typeof(GenericListNotifier<>));

//theme
builder.Services.AddMudServices(config => {
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

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
   
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//auth
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



//Map controllers
app.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");
app.Run();
