using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models
{
    public class ReturnIngredients
    {
        public ReturnIngredients()
        {
            Ingredients = new List<string>();           
        }

        public string Item { get; set; }
        public List<string> Ingredients { get; set; }
        public string Error { get; set; }
    }
}
