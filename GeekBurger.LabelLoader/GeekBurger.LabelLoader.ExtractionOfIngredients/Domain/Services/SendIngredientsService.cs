using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Models;
using Microsoft.Azure.Management.ServiceBus.Fluent;
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
        private readonly IConfigurationRoot _configuration;
        private readonly string _queuename;
        private readonly string _connectionString;
        public SendIngredientsService()
        {
            _configuration = GetConfigurationRoot();
            _queuename = _configuration.GetConnectionString("serviceBus:queueName"); ;
            _connectionString = _configuration.GetConnectionString("serviceBus:connectionString");
        }
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
            CreateQueueIfNotExists();
            var queueClient = new QueueClient(_connectionString, _queuename);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);
        }

        private void CreateQueueIfNotExists()
        {
            var _namespace = _configuration.Get<IServiceBusNamespace>();
            if (!_namespace.Queues.List().Any(x => x.Name.Equals(_queuename)))
                _namespace.Queues.Define(_queuename).WithSizeInMB(1024).Create();
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
