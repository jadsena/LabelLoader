using System;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
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
       /// Extrai os ingredientes
       /// </summary>
       /// <param name="addLabelImage"></param>
       /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LabelLoaderIngredients(AddLabelImage addLabelImage)
        {
            try
            {
                LabelImageAdded labelImageAdded = new LabelImageAdded(){
                    ItemName = addLabelImage.ItemName,
                    Ingredients = await _extractIngredientsService.GetIngredients(addLabelImage.File)            
                };

                _sendIngredientsService.SendIngredients(labelImageAdded);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }           
        }
    }
}