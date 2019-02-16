using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces
{
    public interface IOCRService
    {
        Task<string> CognitiveVisionOCR(byte[] image);
    }
}