using BookStoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.AddSession(options =>
{
	// change idle timeout to 5 minutes - default is 20 minutes
	options.IdleTimeout = TimeSpan.FromDays(30); // Set session timeout to 30 days
	options.Cookie.HttpOnly = false;     // default is true
    options.Cookie.IsEssential = true;   // default is false
});



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookstoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreContext")));

builder.Services.AddIdentity<User, IdentityRole>(
options => {
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<BookstoreContext>()
  .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
