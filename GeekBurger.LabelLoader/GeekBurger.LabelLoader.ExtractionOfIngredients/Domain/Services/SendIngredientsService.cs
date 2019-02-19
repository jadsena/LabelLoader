using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Base;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Services
{
    public class SendIngredientsService : ISendIngredientsService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceBusNamespace _serviceBusNamespace;
        private readonly string _queuename;
        private readonly string _connectionString;

        public SendIngredientsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceBusNamespace = _configuration.GetServiceBusNamespace(); 
            _queuename = _configuration.GetSection("serviceBus:queueName").Value;
            _connectionString = _configuration.GetSection("serviceBus:connectionString").Value;
        }

        public void SendIngredients(LabelImageAdded labelImageAdded)
        {
            try
            {
                string messageBody = JsonConvert.SerializeObject(labelImageAdded);
                SendMessagesAsync(messageBody).Wait();
            }
            catch (Exception ex)
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

            if (!_serviceBusNamespace.Queues.List().Any(x => x.Name.Equals(_queuename)) &&
                !_serviceBusNamespace.Topics.List().Any(x => x.Name.Equals(_queuename)))
                _serviceBusNamespace.Queues.Define(_queuename).WithSizeInMB(1024).Create();
        }       
    }
}
