using DOANCOSO26.Data;
using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using DOANCOSO26.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure Identity options here if needed
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBusRepository, EFBusRepository>();
builder.Services.AddScoped<IBusTripRepository, EFBusTripRepository>();
builder.Services.AddScoped<ISeatRepository, EFSeatRepository>();
builder.Services.AddScoped<IStopRepository, EFStopRepository>();
builder.Services.AddScoped<IBookingRepository, EFBookingRepository>();
builder.Services.AddScoped<IBusRouteRepository, EFBusRouteRepository>();
builder.Services.AddScoped<ITripReportRepository, EFTripReportRepository>();
builder.Services.AddScoped<IDriverregisRepository, EFDriverregisRepository>();
builder.Services.AddSingleton<IVnPaySevices, VnPaySevices>();
builder.Services.AddSingleton<InvoiceCodeGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Bus}/{action=DashBroad}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
