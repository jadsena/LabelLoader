using GeekBurger.LabelLoader.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private string DirectoryName { get; }
        private List<string> Extensoes { get; }
        private FileSystemWatcher Watcher { get; set; }
        private IConfiguration Config { get; }
        private string _urlBase { get; }
        private ILoggerFactory LoggerFactory { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("LabelLoader 1.0.0");
            Program p = new Program();
            p.Run();
            p.ProcessarArquivosParados();
            Console.ReadKey();
        }

        public Program()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            LoggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug()
                .AddFile(@"c:\temp\Log\Log-{Date}.log");
            ILogger logger = LoggerFactory.CreateLogger<Program>();

            DirectoryName = Config.GetSection("LabelImagens:Diretorio").Value;
            Extensoes = Config.GetSection("LabelImagens:Extensoes").Get<List<string>>();
            _urlBase = Config.GetSection("LabelImagens:UrlBase").Value;
            Console.WriteLine($"Diretório para imagens: {DirectoryName}");
            Console.WriteLine($"Extensoes das imagens:  {string.Join(",", Extensoes)}");
            logger.LogInformation($"Diretório para imagens: {DirectoryName}");
            logger.LogInformation($"Extensoes das imagens:  {string.Join(",",Extensoes)}");
            if (!Directory.Exists(DirectoryName)) Directory.CreateDirectory(DirectoryName);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            Watcher = new FileSystemWatcher
            {
                Path = DirectoryName,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName
            };
            Watcher.Created += OnChanged;
            Watcher.EnableRaisingEvents = true;
        }

        public void ProcessarArquivosParados()
        {
            string[] Files = Directory.GetFiles(DirectoryName);
            string temp = "";
            foreach (var item in Files)
            {
                temp = $@"C:\temp\{Path.GetFileName(item)}";
                File.Move(item, temp);
                File.Move(temp, item);
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!Extensoes.Contains(Path.GetExtension(e.Name).ToLower().Replace(".", ""))) return;


            await EnviarParaApi(new FileInfo(e.FullPath));

            Console.WriteLine($"Arquivo: {e.Name}, FullPath: {e.FullPath}, ChangeType: {e.ChangeType}");
            if (!Extensoes.Contains(Path.GetExtension(e.Name).ToLower().Replace(".",""))) return;
            ILogger<Program> logger = LoggerFactory.CreateLogger<Program>();
            logger.LogInformation($"Arquivo: {e.Name}, FullPath: {e.FullPath}, ChangeType: {e.ChangeType}");
        }

        private Uri Url(string url)
        {
            var endpoint = new Uri(new Uri(_urlBase), url);
            return endpoint;
        }

        private async Task<bool> EnviarParaApi(FileInfo arquivo)
        {
            try
            {
                var arquivoConvertido = File.ReadAllBytes(arquivo.FullName);
                AddLabelImage imagem = new AddLabelImage
                {
                    ItemName = arquivo.Name,
                    File = Convert.ToBase64String(arquivoConvertido)
                };

               
                using (var client = new HttpClient())
                {
                    var resposta = await client.PostAsJsonAsync(Url("api/Ingredient"), imagem);
                    if (resposta.IsSuccessStatusCode)
                    {

                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
