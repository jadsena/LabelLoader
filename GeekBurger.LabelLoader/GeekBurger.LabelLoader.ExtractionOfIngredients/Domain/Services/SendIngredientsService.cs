using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services
{
    public class SendIngredientsService : ISendIngredientsService
    {
        public void SendIngredients(LabelImageAdded labelImageAdded)
        {
            try
            {
                string messageBody = JsonConvert.SerializeObject(labelImageAdded);
                SendMessagesAsync(messageBody).Wait();
            }
            catch (Exception)
            {
                throw new Exception("Falha ao enviar ingredientes!");
            }
        }

        private async Task SendMessagesAsync(string messageBody)
        {

            var _config = GetConfigurationRoot();
            string serviceBusConnectionString = _config.GetConnectionString("serviceBus:connectionString");
            string qeueName = _config.GetConnectionString("serviceBus:subscriptionId");
            var queueClient = new QueueClient(serviceBusConnectionString, qeueName);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);

        }

        private IConfigurationRoot GetConfigurationRoot()
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
            return builder.Build();

        }
    }
}
