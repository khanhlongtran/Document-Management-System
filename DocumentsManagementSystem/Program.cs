using DocumentsManagementSystem.Filter;
using DocumentsManagementSystem.Hubs;
using DocumentsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentsManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthenticationFilter());
            });
            // Add SignalR
            builder.Services.AddSignalR();
            //config session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddDbContext<LoginLogoutExampleContext>(options =>
            options.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DB")));
            builder.Services.AddScoped(typeof(LoginLogoutExampleContext));
            // end config
            var app = builder.Build();
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default1",
                pattern: "{controller}/{action}/{id}");
            app.MapControllerRoute(
                name: "default2",
                pattern: "{controller}/{action}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<EditorHub>("/editHub");
            });
            app.Run();
        }
    }
}