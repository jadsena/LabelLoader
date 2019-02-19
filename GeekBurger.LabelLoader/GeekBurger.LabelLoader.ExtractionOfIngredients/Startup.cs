using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Serilog;
using Serilog.Configuration;
using Serilog.Settings.Configuration;
using Swashbuckle.AspNetCore.Swagger;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IExtractIngredientsService, ExtractIngredientsService>();
            services.AddScoped<ISendIngredientsService, SendIngredientsService>();
            services.AddScoped<IOCRService, OCRService>();

            services.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new Info { Title = "Label Loader", Version = "v1" });
             });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Label Loader V1");
            });

            loggerFactory.AddSerilog();
            loggerFactory.AddAzureWebAppDiagnostics(
                new AzureAppServicesDiagnosticsSettings
                {
                    OutputTemplate = "{ Timestamp:yyyy - MM - dd HH:mm:ss.fff zzz } [{ Level }] { Message} { NewLine} { Exception} "
                });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
