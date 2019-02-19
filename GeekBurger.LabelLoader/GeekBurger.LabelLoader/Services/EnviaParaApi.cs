using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class EnviaParaApi : IEnviaParaApi
    {
        private LabelImagesOptions LabelImagesOptions { get; }
        private ILogger Logger { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        public EnviaParaApi(ILogger<EnviaParaApi> logger, IOptions<LabelImagesOptions> options, IHttpClientFactory httpClientFactory)
        {
            Logger = logger;
            LabelImagesOptions = options.Value;
            HttpClientFactory = httpClientFactory;
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


                using (var client = HttpClientFactory.CreateClient("ApiAzure"))
                {
                    Logger.LogInformation($"Enviando arquivo para: {Url("api/Ingredient")}");
                    System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();
                    var resposta = await client.PostAsJsonAsync("api/Ingredient", imagem, cancellationToken);
                    if (resposta.IsSuccessStatusCode)
                    {
                        Logger.LogInformation($"Arquivo [{arquivo.FullName}] enviado com sucesso para: {Url("api/Ingredient")}. StatusCode {(int)resposta.StatusCode} - {resposta.StatusCode}");
                        return true;

                    }
                    else
                    {
                        Logger.LogError($"Arquivo [{arquivo.FullName}] não enviado para: {Url("api/Ingredient")}. Error: {Convert.ToInt32(resposta.StatusCode)} - {resposta.StatusCode}");
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
