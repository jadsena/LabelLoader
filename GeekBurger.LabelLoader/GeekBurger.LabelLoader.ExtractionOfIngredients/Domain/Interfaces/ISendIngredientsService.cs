using GeekBurger.LabelLoader.Contract;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces
{
    public interface ISendIngredientsService
    {
        void SendIngredients(LabelImageAdded labelImageAdded);
    }
}