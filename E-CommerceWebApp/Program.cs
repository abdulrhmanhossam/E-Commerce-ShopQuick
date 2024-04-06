using DataBaseAccess;
using E_CommerceWebApp.EmailServiceUsingFluent;
using E_CommerceWebApp.Utility;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddTransient<IFluentEmailService, FluentEmailService>();

var outlookSender = builder.Configuration.GetSection("Outlook")["Sender"];
var outlookPassword = builder.Configuration.GetSection("Outlook")["Password"];
var outlookHost = builder.Configuration.GetSection("Outlook")["Host"];
var outlookPort = Convert.ToInt32(builder.Configuration.GetSection("Outlook")["Port"]);

builder.Services.AddFluentEmail(outlookSender)// Sender Info
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient(outlookHost)// using outlook smtp as client 
    {
        UseDefaultCredentials = true,
        Port = outlookPort,
        Credentials = new NetworkCredential(outlookSender, outlookPassword), // Sender Info
        EnableSsl = true
    });


builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
