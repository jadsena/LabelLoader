using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models
{
    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class ImageInfoViewModel
    {
        public string language { get; set; }
        public string orientation { get; set; }
        public int textAngle { get; set; }
        public List<Region> regions { get; set; }
    }

    public class ReturnIngredients
    {
        public ReturnIngredients()
        {
            Ingredients = new List<string>();
            Erros = new List<string>();
        }

        public string Item { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Erros { get; set; }
    }
}
