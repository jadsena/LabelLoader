using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Options;
using GeekBurger.LabelLoader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;


namespace GeekBurger.LabelLoader
{
    class Program
    {
        private IConfiguration Config { get; set; }
        private ILogger<Program> Logger { get; set; }
        private ILerDiretorio LerDiretorio { get; }
        static void Main(string[] args)
        {
            Console.WriteLine("LabelLoader 1.0.0");
            Program p = new Program();
            p.Run();
            Console.ReadKey();
        }

        public Program()
        {
            Configure();
            
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            Logger = serviceProvider.GetService< ILoggerFactory>().CreateLogger<Program>();
            LabelImagesOptions labelImages = serviceProvider.GetService<IOptions<LabelImagesOptions>>().Value;

            Logger.LogInformation("Configurações:");
            Logger.LogInformation(labelImages.ToString());
            if (!Directory.Exists(labelImages.Diretorio)) Directory.CreateDirectory(labelImages.Diretorio);
            if (!Directory.Exists(labelImages.Processados)) Directory.CreateDirectory(labelImages.Processados);
            LerDiretorio = serviceProvider.GetService<ILerDiretorio>();
        }

        private void Configure()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory>( new LoggerFactory()
                .AddConsole()
                .AddDebug()
                .AddFile(@"c:\temp\Log\Log-{Date}.log"));
            services.AddLogging();
            services.AddOptions();
            services.Configure<LabelImagesOptions>(Config.GetSection("LabelImagesOptions"));
            services.AddTransient<ILerDiretorio, LerDiretorio>();
            services.AddTransient<IEnviaParaApi, EnviaParaApi>();
            
            services.AddHttpClient("ApiAzure", client => 
            {
                client.BaseAddress = new Uri(Config.GetValue<string>("LabelImagesOptions:UrlBase"));
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        }

        public void Run()
        {
            LerDiretorio.Run();
        }
    }
}
