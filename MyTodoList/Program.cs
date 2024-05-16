using MyTodoList.Data;
using MyTodoList.Interfaces;
using MyTodoList.Repositories;

namespace MyTodoList;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IJobRepository, JobRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                 throw new Exception("Connection string is not valid!");
        
        var databaseService = new DatabaseService(connectionString);
        builder.Services.AddSingleton(databaseService);

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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Todo}/{action=Todo}/{id?}");

        app.Run();
    }
}
