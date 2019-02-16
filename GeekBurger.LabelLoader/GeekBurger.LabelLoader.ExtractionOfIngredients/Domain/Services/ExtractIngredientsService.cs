using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly IOCRService _ocrService;

        public ExtractIngredientsService(IOCRService ocrService)
        {
            _ocrService = ocrService;
        }

        public async Task<List<string>> GetIngredients(string imageBase64)
        {
            try
            {
                byte[] image = Convert.FromBase64String(imageBase64);

                var result = await _ocrService.CognitiveVisionOCR(image);
                result = result.Replace("Ingredientes", "", StringComparison.OrdinalIgnoreCase);                

                return result.Split(",").ToList();
            }
            catch (Exception)
            {
                throw new Exception("Falha ao extrair ingredientes");
            }
        }
    }
}
