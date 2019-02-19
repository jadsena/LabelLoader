using System;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IExtractIngredientsService _extractIngredientsService;
        private readonly ISendIngredientsService _sendIngredientsService;
        private readonly ILogger _logger;

        public IngredientController(IExtractIngredientsService extractIngredients, ISendIngredientsService sendIngredients, ILogger logger)
        {
            _extractIngredientsService = extractIngredients;
            _sendIngredientsService = sendIngredients;
            _logger = logger;
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
                _logger.Information($"Iniciando extração de ingredientes item {addLabelImage.ItemName}," +
                    $" imagem {addLabelImage.ItemName }");

                LabelImageAdded labelImageAdded = new LabelImageAdded(){
                    ItemName = addLabelImage.ItemName,
                    Ingredients = await _extractIngredientsService.GetIngredients(addLabelImage.File)            
                };

                _sendIngredientsService.SendIngredients(labelImageAdded);

                _logger.Information("Extração finalizada");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"Falha no processo de extração de ingredientes {ex.Message}");
                return NotFound();
            }           
        }
    }
}