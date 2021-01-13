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
            //Dependency Injection ayarlar�
            //E�er constructor da IRepository ile kar��la��rsan git Reepository den bir nesne �rne�i olu�tur ve IRepository e ata.

            //AddScoped �n yapt��� i�lem: Bir istek esnas�nda, bir class�n ctor unda bir interface ile kar��la��rsa, ona kar��l�k gelen classa giderek bir nesne �rne�i olu�turur.
            //Ayn� reques i�erisinde birden fazla kar��la��rsa ilk olu�tudu�u nesne �rne�ini al�r.
            //E�er her kar��la�t���nda yeni bir nesne �rne�i olu�turmas� i�in "AddTransient" i kullanabiliriz. Ama AddScoped daha performansl�d�r.
            // "AddSingleton" ise sadece tek birkez olu�turur ve uygulama boyunca olu�an o class � kullan�r.

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IService<>), typeof(Service.Services.Service<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //EntityFramework �n kullanaca�� veritaban�(splserver) ve ba�lant�s� verildi.
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
