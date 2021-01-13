using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyNLayerProject.Core.Repositories;
using UdemyNLayerProject.Core.Services;
using UdemyNLayerProject.Core.UnitOfWorks;
using UdemyNLayerProject.Data;
using UdemyNLayerProject.Data.Repositories;
using UdemyNLayerProject.Data.UnitOfWorks;
using UdemyNLayerProject.Service.Services;
using UdemyNLayerProject.Web.ApiService;
using UdemyNLayerProject.Web.Filters;

namespace UdemyNLayerProject.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<CategoryApiService>(opt =>
            {
                opt.BaseAddress = new Uri(Configuration["baseUrl"]);//appsettings e Configuration ile ulaþýlýr.
            });

            services.AddScoped<NotFoundFilter>();
            services.AddAutoMapper(typeof(Startup));
            //Dependency Injection ayarlarý
            //Eðer constructor da IRepository ile karþýlaþýrsan git Reepository den bir nesne örneði oluþtur ve IRepository e ata.

            //AddScoped ýn yaptýðý iþlem: Bir istek esnasýnda, bir classýn ctor unda bir interface ile karþýlaþýrsa, ona karþýlýk gelen classa giderek bir nesne örneði oluþturur.
            //Ayný reques içerisinde birden fazla karþýlaþýrsa ilk oluþtuduðu nesne örneðini alýr.
            //Eðer her karþýlaþtýðýnda yeni bir nesne örneði oluþturmasý için "AddTransient" i kullanabiliriz. Ama AddScoped daha performanslýdýr.
            // "AddSingleton" ise sadece tek birkez oluþturur ve uygulama boyunca oluþan o class ý kullanýr.
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IService<>), typeof(Service.Services.Service<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //EntityFramework ün kullanacaðý veritabaný(splserver) ve baðlantýsý verildi.
                options.UseSqlServer(Configuration
                    ["ConnectionStrings:SqlConStr"].ToString(), o =>
                    {
                        o.MigrationsAssembly("UdemyNLayerProject.Data");
                    });
            });


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
