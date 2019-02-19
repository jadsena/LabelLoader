using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services
{
    public class OCRService : IOCRService
    {
        readonly private IConfiguration _configuration;
        private readonly ILogger _logger;

        public OCRService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> CognitiveVisionOCR(byte[] image)
        {
            try
            {
                string result = string.Empty;
                ImageInfoViewModel responeData = new ImageInfoViewModel();
                HttpClient client = new HttpClient();
                HttpResponseMessage response;

                _logger.Information("Inicia a request");
               
                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.GetSection("Cognitive:SubscriptionKey").Value);


                _logger.Information("Prepara o content");
                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(image))
                {
                    //"application/octet-stream" content type.                  
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync($"{_configuration.GetSection("Cognitive:UriBase").Value}?{_configuration.GetSection("Cognitive:UriParameters").Value}", content);
                }

                _logger.Information("Acessa o serviço de OCR");
                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("Retorno com sucesso");

                    // The JSON response mapped into respective view model.  
                    responeData = JsonConvert.DeserializeObject<ImageInfoViewModel>(contentString,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Include,
                                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                                {
                                    earg.ErrorContext.Handled = true;
                                }
                            }
                        );

                    _logger.Information("Retorno deserializado");

                    var linesCount = responeData.regions[0].lines.Count;
                    for (int i = 0; i < linesCount; i++)
                    {
                        var wordsCount = responeData.regions[0].lines[i].words.Count;
                        for (int j = 0; j < wordsCount; j++)
                        {
                            //Concatenate only the text property                           
                            result += responeData.regions[0].lines[i].words[j].text + " ";
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {
                _logger.Error("Falha no OCR");
                throw new Exception("Falha no OCR");
            }
        }
    }
}
