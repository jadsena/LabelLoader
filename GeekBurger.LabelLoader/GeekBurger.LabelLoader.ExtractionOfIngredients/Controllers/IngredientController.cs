using System;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IExtractIngredientsService _extractIngredients;

        public IngredientController(IExtractIngredientsService extractIngredients)
        {
            _extractIngredients = extractIngredients;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ReturnIngredients> GetIngredients(string item, string imageBase64)
        {
            try
            {
                ReturnIngredients returnIngredients = new ReturnIngredients(){
                    Item = item,
                    Ingredients = await _extractIngredients.GetIngredients(imageBase64)            
                };

                return returnIngredients;

            }
            catch (Exception e)
            {
                return new ReturnIngredients() { Error = "Ocorreu erro ao extrair os ingredientes!"};
            }           
        }
    }
}