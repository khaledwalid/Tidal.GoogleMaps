using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tidal.GoogleMaps.Business.Services;
using Tidal.GoogleMaps.Contracts.Contracts;
using Tidal.GoogleMaps.DB.Context;
using Tidal.GoogleMaps.DB.Models;
using Tidal.GoogleMaps.Infrastructure.Repositories;
using Tidal.GoogleMaps.Models.Mapper;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Tidal.GoogleMaps
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // In ASP.NET Core 3.0 env will be an IWebHostEnvironment , not IHostingEnvironment.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Startup.Configuration = builder.Build();
        }
        public static IConfigurationRoot Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection
            services.AddOptions();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc();
            services.AddEntityFrameworkNpgsql().AddDbContext<GoogleMapsDbContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("GoogleMapsConnection")));
            services.AddAutoMapper(typeof(MapMapperConfiguration));

            var builder = new ContainerBuilder();
            ConfigureContainer(builder);
            builder.Populate(services);
            AutofacContainer = builder.Build();

            // this will be used as the service-provider for the application!
            return new AutofacServiceProvider(AutofacContainer);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<MapService>().As<IMapService>();
            builder.RegisterType<MapRepository>().As<IMapRepository>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .AsClosedTypesOf(typeof(IRepository<>), true)
                .AsImplementedInterfaces();
            builder.RegisterModule<AutoFacModule>();

            builder
                .RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();
            builder
                .RegisterType<GoogleMapsDbContext>()
                .WithParameter("options", DbContextOptionsFactory.Get())
                .InstancePerLifetimeScope();
            //builder.RegisterType<MapMapperConfiguration>().As<Profile>();

            //RegisterMaps(builder);
        }
        public class DbContextOptionsFactory
        {
            public static DbContextOptions<GoogleMapsDbContext> Get()
            {

                var builder = new DbContextOptionsBuilder<GoogleMapsDbContext>();
                DbContextConfiguration.Configure(
                    builder,
                   Configuration.GetConnectionString("GoogleMapsConnection"));

                return builder.Options;
            }
        }
        public class DbContextConfiguration
        {
            public static void Configure(
                DbContextOptionsBuilder<GoogleMapsDbContext> builder,
                string connectionString)
            {
                builder.UseNpgsql(connectionString);
            }
        }
        private static void RegisterMaps(ContainerBuilder builder)
        {
            var assemblyNames = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyName = assemblyNames
                .Where(a => a.FullName.Contains("Tidal.GoogleMaps.Models", StringComparison.OrdinalIgnoreCase));
            var assembliesTypes = assemblyName
                .SelectMany(an => Assembly.Load(an.FullName).GetTypes())
                .Where(p => typeof(Profile).IsAssignableFrom(p) && p.IsPublic && !p.IsAbstract)
                .Distinct();
            var autoMapperProfiles = assembliesTypes
                .Select(p => (Profile)Activator.CreateInstance(p)).ToList();
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in autoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , ILoggerFactory loggerFactory)
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

            //loggerFactory.AddDebug();

        }
    }
}
