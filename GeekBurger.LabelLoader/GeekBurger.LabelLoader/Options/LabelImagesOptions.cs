using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Options
{
    public class LabelImagesOptions
    {
        public string Diretorio { get; set; }
        public string Processados { get; set; }
        public List<string> Extensoes { get; set; }
        public string UrlBase { get; set; }

        public override string ToString()
        {
            return $"Diretorio: {Diretorio}\n" +
                   $"Processados: {Processados}\n"+
                   $"Extensoes: [{string.Join(",", Extensoes)}]\n"+
                   $"UrlBase: {UrlBase}";
        }
    }
}
