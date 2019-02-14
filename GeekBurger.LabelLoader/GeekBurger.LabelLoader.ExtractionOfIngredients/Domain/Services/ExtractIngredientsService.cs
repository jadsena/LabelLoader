using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services
{
    public class ExtractIngredientsService : IExtractIngredientsService
    {       
        public async Task<List<string>> GetIngredients (string imageBase64)
        {            
            ImageInfoViewModel responeData = new ImageInfoViewModel();
            HttpClient client = new HttpClient();
            HttpResponseMessage response;

            string result = "";
            string subscriptionKey = "c7ca131650e84ef0a64a903a54867f5c";
            string uriBase = "https://centralus.api.cognitive.microsoft.com/vision/v2.0/ocr";
            string requestParameters = "language=pt&detectOrientation=true";

            byte[] image = Convert.FromBase64String(imageBase64);

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);                           

            // Add the byte array as an octet stream to the request body.
            using (ByteArrayContent content = new ByteArrayContent(image))
            {
                //"application/octet-stream" content type.                  
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.
                response = await client.PostAsync($"{uriBase}?{requestParameters}", content);
            }

            // Asynchronously get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
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

                var linesCount = responeData.regions[0].lines.Count;
                for (int i = 0; i < linesCount; i++)
                {
                    var wordsCount = responeData.regions[0].lines[i].words.Count;
                    for (int j = 0; j < wordsCount; j++)
                    {
                        //Concatenate only the text property
                        if (responeData.regions[0].lines[i].words[j].text.ToUpper() != "INGREDIENTES")
                            result += responeData.regions[0].lines[i].words[j].text + " ";
                    }
                }
            }

            return result.Split(",").ToList();
        }
    }
}
