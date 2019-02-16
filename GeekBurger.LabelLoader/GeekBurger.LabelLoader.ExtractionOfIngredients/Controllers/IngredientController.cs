using System;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IExtractIngredientsService _extractIngredientsService;
        private readonly ISendIngredientsService _sendIngredientsService;

        public IngredientController(IExtractIngredientsService extractIngredients, ISendIngredientsService sendIngredients)
        {
            _extractIngredientsService = extractIngredients;
            _sendIngredientsService = sendIngredients;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<LabelImageAdded> LabelLoaderIngredients(AddLabelImage addLabelImage)
        {
            try
            {
                LabelImageAdded labelImageAdded = new LabelImageAdded(){
                    ItemName = addLabelImage.ItemName,
                    Ingredients = await _extractIngredientsService.GetIngredients(addLabelImage.File)            
                };

                _sendIngredientsService.SendIngredients(labelImageAdded);

                return labelImageAdded;
            }
            catch (Exception)
            {
                return new LabelImageAdded();
            }           
        }
    }
}