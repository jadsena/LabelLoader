using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Base;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IExtractIngredientsService _extractIngredientsService;
        private readonly ISendIngredientsService _sendIngredientsService;
        private readonly ILogger<IngredientController> _logger;

        public IngredientController(IExtractIngredientsService extractIngredients, ISendIngredientsService sendIngredients, ILogger<IngredientController> logger)
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
                _logger.LogInformation($"Iniciando extração de ingredientes item {addLabelImage.ItemName}," +
                    $" imagem {addLabelImage.ItemName }");

                if (!ValidFile(addLabelImage.File))
                    return NotFound(":(");

                    LabelImageAdded labelImageAdded = new LabelImageAdded()
                    {
                        ItemName = addLabelImage.ItemName,
                        Ingredients = await _extractIngredientsService.GetIngredients(addLabelImage.File)
                    };

                    _sendIngredientsService.SendIngredients(labelImageAdded);

                    _logger.LogInformation("Extração finalizada");

                    return Ok();
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Falha no processo de extração de ingredientes {ex.ToString()}");
                return NotFound();
            }           
        }

        

        private bool ValidFile(string imageBase64)
        {
            byte[] image = Convert.FromBase64String(imageBase64);
            var formato = Helper.GetImageFormat(image);

            if (formato == ImageFormat.unknown || image.Length == 0)
            {
                return false;
            }
            return true;
        }
    }
}