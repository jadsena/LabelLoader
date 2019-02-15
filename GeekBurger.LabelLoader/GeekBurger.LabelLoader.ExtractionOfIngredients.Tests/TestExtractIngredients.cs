using Autofac.Extras.FakeItEasy;
using FakeItEasy;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using System.Collections.Generic;
using System;
using System.IO;
using Xunit;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Tests
{
    public class TestExtractIngredients
    {
        [Fact]
        public void GetIngredientsComSucesso()
        {
            string imagemBase64 = string.Empty;
            var directory = Directory.GetCurrentDirectory();
            var path = $"{directory}GeekBurger.LabelLoader/ImagensTeste/produto.jpg";

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                var imageByte = binaryReader.ReadBytes((int)fileStream.Length);
                imagemBase64 = Convert.ToBase64String(imageByte);
            }

            var ingredientsFake = A.Fake<List<string>>();
            var _mocker = new AutoFake();
            var service = _mocker.Resolve<IExtractIngredientsService>();

            A.CallTo(() => service.GetIngredients(imagemBase64)).Returns(ingredientsFake); //MustHaveHappened();
        }

        [Fact]
        public void GetIngredientsComErro()
        {
            string imagemBase64 = string.Empty;         

            var _mocker = new AutoFake();
            var service = _mocker.Resolve<IExtractIngredientsService>();

            var exception = Record.ExceptionAsync(() => service.GetIngredients(imagemBase64));
            Assert.IsType<ArgumentNullException>(exception);

            var ex = Assert.ThrowsAsync<Exception>(() =>  service.GetIngredients(imagemBase64)).Result;
            Assert.Equal("Falha ao extrair ingredientes", ex.Message);
        }
    }
}
