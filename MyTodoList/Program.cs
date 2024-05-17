using MyTodoList.Data;
using MyTodoList.Data.Service;
using MyTodoList.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<JobRepositorySql>();
builder.Services.AddTransient<JobRepositoryXml>();

builder.Services.AddTransient<JobRepositorySwitcher>(sp => new JobRepositorySwitcher(
    sp.GetRequiredService<JobRepositorySql>(),
    sp.GetRequiredService<JobRepositoryXml>(),
    sp.GetRequiredService<IHttpContextAccessor>(),
    sp.GetRequiredService<ILogger<JobRepositorySwitcher>>()
));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                         throw new Exception("Connection string is not valid!");

var databaseService = new DatabaseService(connectionString);
builder.Services.AddSingleton(databaseService);

var xmlFilesDirectory = Path.Combine(
    Directory.GetCurrentDirectory(), builder.Configuration.GetValue<string>("XmlFilesDirectory") ?? 
                                     throw new Exception("XmlFilesDirectory is not valid!"));
builder.Services.AddSingleton(new XmlStorageService(xmlFilesDirectory));

// Add logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add this line

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Todo}/{id?}");

app.Run();
