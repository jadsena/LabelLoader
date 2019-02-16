using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

namespace GeekBurger.LabelLoader
{
    class Program
    {
        private string DirectoryName { get; }
        private List<string> Extensoes { get; }
        private FileSystemWatcher Watcher { get; set; }
        private IConfiguration Config { get; }
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
            DirectoryName = Config.GetSection("LabelImagens:Diretorio").Value;
            Extensoes = Config.GetSection("LabelImagens:Extensoes").Get<List<string>>();
            Console.WriteLine($"Diretório para imagens: {DirectoryName}");
            Console.WriteLine($"Extensoes das imagens:  {string.Join(",",Extensoes)}");
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

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!Extensoes.Contains(Path.GetExtension(e.Name).ToLower().Replace(".",""))) return;
            Console.WriteLine($"Arquivo: {e.Name}, FullPath: {e.FullPath}, ChangeType: {e.ChangeType}");
        }
    }
}
