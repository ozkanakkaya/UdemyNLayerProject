using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
using AutoMapper;
using UdemyNLayerProject.API.Filters;
using UdemyNLayerProject.API.Extensions;

namespace UdemyNLayerProject.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.//Servislerimizi eklediðimiz method
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));//tüm nesnelerimizin dönüþtürme iþlemini gerçekleþtirir

            services.AddScoped<NotFoundFilter>();//bu filter dependency injection nesnesi aldýðýndan dolayý buraya eklememiz gerekti.

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

            services.AddScoped<IUnitOfWork,UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //EntityFramework ün kullanacaðý veritabaný(splserver) ve baðlantýsý verildi.
                options.UseSqlServer(Configuration
                    ["ConnectionStrings:SqlConStr"].ToString(), o =>
                     {
                         o.MigrationsAssembly("UdemyNLayerProject.Data");
                     });
            });

            services.AddControllers(o=> 
            {
                o.Filters.Add(new ValidationFilter());//global düzeyde bu filterýmýzý tüm controllerýmýza eklemiþ olduk.
            });

            //Kendi errorlarýmýzý yapabilmemiz için
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;//filterlarý kontro etmemesi..
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. (Katmanlarý eklediðimiz method
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomException();//bizim yazdýðmýz extensions(sistemdeki var olan hata yakalamalara eklediðimizi ifade eder) metodumuz

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
