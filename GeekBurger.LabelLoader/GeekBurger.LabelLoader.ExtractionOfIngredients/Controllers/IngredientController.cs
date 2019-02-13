using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ReturnIngredients> ExtractIngredients(string item, string imageBase64)
        {
            ReturnIngredients returnIngredients = new ReturnIngredients();

            try
            {                
                string extractedResult = "";
                ImageInfoViewModel responeData = new ImageInfoViewModel();
                var subscriptionKey = "c7ca131650e84ef0a64a903a54867f5c";
                /// vision / v2.0 / analyze
                var uriBase = "https://centralus.api.cognitive.microsoft.com/vision/v2.0/ocr";
                HttpClient client = new HttpClient();
                HttpResponseMessage response;

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters               
                string requestParameters = "language=pt&detectOrientation=true";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;
                byte[] image = Convert.FromBase64String(imageBase64);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(image))
                {
                    //"application/octet-stream" content type.                  
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
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
                                returnIngredients.Erros.Add(earg.ErrorContext.Member.ToString());
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
                            //Appending all the lines content into one.  
                            if (responeData.regions[0].lines[i].words[j].text.ToUpper() != "INGREDIENTES")
                                extractedResult += responeData.regions[0].lines[i].words[j].text + " ";
                        }     
                    }
                }

                returnIngredients.Item = item;
                returnIngredients.Ingredients = extractedResult.Split(",").ToList();

                return returnIngredients;
            }
            catch (Exception e)
            {
                return returnIngredients;
            }           
        }
    }
}