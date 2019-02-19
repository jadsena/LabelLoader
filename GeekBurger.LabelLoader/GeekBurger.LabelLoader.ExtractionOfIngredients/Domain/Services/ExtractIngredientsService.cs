using GeekBurger.LabelLoader.ExtractionOfIngredients.Base;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
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
        private readonly IOCRService _ocrService;
        private readonly ILogger<ExtractIngredientsService> _logger;

        public ExtractIngredientsService(IOCRService ocrService, ILogger<ExtractIngredientsService> logger)
        {
            _ocrService = ocrService;
            _logger = logger;
        }

        public async Task<List<string>> GetIngredients(string imageBase64)
        {
            try
            {
                byte[] image = Convert.FromBase64String(imageBase64);
              
                _logger.LogInformation("Passou pela validação");

                var result = await _ocrService.CognitiveVisionOCR(image);

                _logger.LogInformation("Passou pelo OCR");

                result = result.Replace("Ingredientes:", "", StringComparison.OrdinalIgnoreCase);
                result = result.Replace("Ingredientes", "", StringComparison.OrdinalIgnoreCase);                

                var ingredients = result.Split(",").ToList();

                ingredients.ForEach( x => {
                    _logger.LogInformation($"{x}");
                });

                return ingredients;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Falha ao extrair ingredientes - {ex.Message}");
                throw new Exception("Falha ao extrair ingredientes");
            }
        }
    }
}
