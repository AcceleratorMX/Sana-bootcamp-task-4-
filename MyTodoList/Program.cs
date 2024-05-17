using MyTodoList.Data.Service;
using MyTodoList.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<JobRepositorySql>();
builder.Services.AddTransient<JobRepositoryXml>();
builder.Services.AddSingleton<RepositoryTypeService>();

builder.Services.AddTransient<JobRepositorySwitcher>(sp => new JobRepositorySwitcher(
    sp.GetRequiredService<JobRepositorySql>(),
    sp.GetRequiredService<JobRepositoryXml>(),
    sp.GetRequiredService<RepositoryTypeService>(),
    sp.GetRequiredService<ILogger<JobRepositorySwitcher>>()
));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new Exception("Connection string is not valid!");

var databaseService = new DatabaseService(connectionString);
builder.Services.AddSingleton(databaseService);

var xmlFilesDirectory = Path.Combine(
    Directory.GetCurrentDirectory(), builder.Configuration.GetValue<string>("XmlFilesDirectory") ?? 
                                     throw new Exception("XmlFilesDirectory is not valid!"));
builder.Services.AddSingleton(new XmlStorageService(xmlFilesDirectory));

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});


builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Todo}/{id?}");

app.Run();