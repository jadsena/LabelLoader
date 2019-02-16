using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class EnviaParaApi : IEnviaParaApi
    {
        public LabelImagesOptions LabelImagesOptions { get; }
        public ILogger Logger { get; }

        public EnviaParaApi(ILogger<EnviaParaApi> logger, IOptions<LabelImagesOptions> options)
        {
            Logger = logger;
            LabelImagesOptions = options.Value;
        }

        public Task<bool> EnviarAsync(FileInfo arquivo)
        {
            return EnviarParaApi(arquivo);
        }
        private Uri Url(string url)
        {
            var endpoint = new Uri(new Uri(LabelImagesOptions.UrlBase), url);
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
                    Logger.LogInformation($"Enviando arquivo para: {Url("api/Ingredient")}");
                    System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();
                    var resposta = await client.PostAsJsonAsync(Url("api/Ingredient"), imagem, cancellationToken);
                    if (resposta.IsSuccessStatusCode)
                    {
                        Logger.LogInformation($"Arquivo [{arquivo.FullName}] enviado com sucesso para: {Url("api/Ingredient")}");
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
                Logger.LogError($"Erro ao enviar o arquivo [{arquivo.FullName}]: {ex.ToString()}");
                throw;
            }
        }


    }
}
