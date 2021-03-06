﻿using GeekBurger.LabelLoader.Helper;
using GeekBurger.LabelLoader.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class LerDiretorio : ILerDiretorio
    {
        private ILogger<LerDiretorio> Logger { get; }
        private LabelImagesOptions LabelImagesOptions { get; }
        private FileSystemWatcher Watcher { get; set; }
        private IEnviaParaApi EnviaParaApi { get; }
        public LerDiretorio(IEnviaParaApi enviaParaApi, ILogger<LerDiretorio> logger, IOptions<LabelImagesOptions> options)
        {
            Logger = logger;
            LabelImagesOptions = options.Value;
            EnviaParaApi = enviaParaApi;
        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            Watcher = new FileSystemWatcher
            {
                Path = LabelImagesOptions.Diretorio,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.FileName
                     | NotifyFilters.DirectoryName
            };
            Watcher.Created += OnChanged;
            Watcher.EnableRaisingEvents = true;
            Logger.LogDebug($"Inicio FileSystemWatcher do diretorio [{LabelImagesOptions.Diretorio}]");
            ProcessarArquivosParados();
        }

        public void ProcessarArquivosParados()
        {
            Logger.LogDebug("Inicio ProcessarArquivosParados");
            string[] Files = Directory.GetFiles(LabelImagesOptions.Diretorio);
            string temp = "";
            foreach (var item in Files)
            {
                temp = $@"C:\temp\{Path.GetFileName(item)}";
                if (File.Exists(temp)) File.Delete(temp);
                File.Move(item, temp);
                File.Move(temp, item);
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!LabelImagesOptions.Extensoes.Contains(Path.GetExtension(e.Name).ToLower().Replace(".", ""))) return;
            Logger.LogInformation($"Arquivo: {e.Name}, FullPath: {e.FullPath}, ChangeType: {e.ChangeType}");
            MoveArquivoProcessado moveArquivoProcessado = null;
            try
            {
                bool bRet = await EnviaParaApi.EnviarAsync(new FileInfo(e.FullPath));
                if(bRet)
                    moveArquivoProcessado = new MoveArquivoProcessado(e.FullPath, LabelImagesOptions);
                else
                    moveArquivoProcessado = new MoveArquivoProcessado(e.FullPath, LabelImagesOptions, new Exception("Falha ao enviar o arquivo."));
                moveArquivoProcessado.Mover();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
                moveArquivoProcessado = new MoveArquivoProcessado(e.FullPath, LabelImagesOptions, ex);
                moveArquivoProcessado.Mover();
            }
        }
    }
}
