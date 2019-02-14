using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces
{
    public interface ISendIngredientsService
    {
        void SendIngredients(ReturnIngredients ingredients);
    }
}