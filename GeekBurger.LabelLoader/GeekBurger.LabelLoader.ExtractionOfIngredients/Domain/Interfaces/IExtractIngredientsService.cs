using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces
{
    public interface IExtractIngredientsService
    {
        Task<List<string>> GetIngredients(string imageBase64);
    }
}
