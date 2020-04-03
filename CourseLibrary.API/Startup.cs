using AutoMapper;
using Siteminder.API.DbContexts;
using Siteminder.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using Siteminder.API.Models;
using Siteminder.API.Services.CustomerAccounts;

namespace Siteminder.API
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

            services.AddHttpCacheHeaders((expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            });

            services.AddResponseCaching();
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.CacheProfiles.Add("240SecondsCacheProfile",
                    new CacheProfile()
                    {
                        Duration = 240
                    });
            })
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                 new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://Siteminder.com/modelvalidationproblem",
                        Title = "One or more model validation errors occurred.",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the errorsproperty for details",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetails.Extensions.Add("traceid", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            //register the PropertyMappingService...
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            // register the property checker service...
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ISiteTypeRepository, SiteTypeRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ITerminalRepository, TerminalRepository>();
            services.AddScoped<ITerminalSettingsRepository, TerminalSettingsRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IFuelTypeRepository, FuelTypesRepository>();
            services.AddScoped<IFuelRepository, FuelRepository>();
            services.AddScoped<IDispenserRepository, DispenserRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ICardTypeRepository, CardTypeRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<ICustomerAccountRepository, CustomerAccountRepository>();


            services.AddDbContext<SiteminderContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Development"));

                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=SiteminderDB;Trusted_Connection=True;");

            });
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
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again Later.");

                        //add logging here....
                    });
                });
            }

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
