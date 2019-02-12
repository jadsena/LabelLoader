using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task ExtractIngredients(string item, string imageBase64)
        {
            try
            {
                var subscriptionKey = "a51bae8b73d54fc8a7c32361b173241f";
                var uriBase = "https://brazilsouth.api.cognitive.microsoft.com/vision/v2.0/analyze";
                HttpClient client = new HttpClient();
                HttpResponseMessage response;

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters               
                string requestParameters = "visualFeatures=Categories,Description,Color";

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

                // Display the JSON response.
                var result = JToken.Parse(contentString).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }
    }
}