using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookShop.Data;
using BookShop.Areas.Identity.Data;
using BookShop.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BookShopContextConnection");


builder.Services.AddDbContext<BookShopContext>(options =>
    options.UseSqlServer(connectionString));;

/*builder.Services.AddDefaultIdentity<BookShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<BookShopContext>();;
;
*/
builder.Services.AddDbContext<BookShopContext>(options =>
    options.UseSqlServer(connectionString));;

builder.Services.AddDefaultIdentity<BookShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BookShopContext>();;

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true; 
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;    

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._@";
    options.User.RequireUniqueEmail = false;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

/* Configure session state */
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var config = builder.Configuration;
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSenderOptions>(options =>
{
    options.Host = config["MailSettings:Host"];
    options.Port = int.Parse(config["MailSettings:Port"]);
    options.User = config["MailSettings:User"];
    options.Pass = config["MailSettings:Pass"];
    options.Name = config["MailSettings:Name"];
    options.Sender = config["MailSettings:User"];
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Customer", "Seller" };
    IdentityResult roleResult;
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}


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
app.UseAuthentication();;

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
