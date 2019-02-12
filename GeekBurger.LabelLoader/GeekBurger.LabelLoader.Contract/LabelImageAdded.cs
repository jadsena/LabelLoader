using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Contract
{
    public class LabelImageAdded
    {
        public string ItemName { get; set; }

        public List<String> Ingredients { get; set; }

    }
}
