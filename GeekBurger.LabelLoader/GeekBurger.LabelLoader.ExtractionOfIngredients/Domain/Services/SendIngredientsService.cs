using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services
{
    public class SendIngredientsService : ISendIngredientsService
    {
        public void SendIngredients(ReturnIngredients ingredients)
        {
            try
            {

            }
            catch (Exception)
            {
                throw new Exception("Falha ao enviar ingredientes!");
            }            
        }
    }
}
