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

        // This method gets called by the runtime. Use this method to add services to the container.//Servislerimizi ekledi�imiz method
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));//t�m nesnelerimizin d�n��t�rme i�lemini ger�ekle�tirir

            services.AddScoped<NotFoundFilter>();//bu filter dependency injection nesnesi ald���ndan dolay� buraya eklememiz gerekti.

            //Dependency Injection ayarlar�
            //E�er constructor da IRepository ile kar��la��rsan git Reepository den bir nesne �rne�i olu�tur ve IRepository e ata.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IService<>), typeof(Service.Services.Service<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            //AddScoped �n yapt��� i�lem: Bir istek esnas�nda, bir class�n ctor unda IUnitOfWork ile kar��la��rsa, UnitOfWork a giderek bir nesne �rne�i alacak.
            services.AddScoped<IUnitOfWork,UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //EntityFramework �n kullanaca�� veritaban�(splserver) ve ba�lant�s� verildi.
                options.UseSqlServer(Configuration
                    ["ConnectionStrings:SqlConStr"].ToString(), o =>
                     {
                         o.MigrationsAssembly("UdemyNLayerProject.Data");
                     });
            });

            services.AddControllers(o=> 
            {
                o.Filters.Add(new ValidationFilter());//global d�zeyde bu filter�m�z� t�m controller�m�za eklemi� olduk.
            });

            //Kendi errorlar�m�z� yapabilmemiz i�in
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;//filterlar� kontro etmemesi..
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. (Katmanlar� ekledi�imiz method
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomException();//bizim yazd��m�z extensions(sistemdeki var olan hata yakalamalara ekledi�imizi ifade eder) metodumuz

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
