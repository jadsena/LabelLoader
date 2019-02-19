using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Base;
using GeekBurger.LabelLoader.ExtractionOfIngredients.Domain.Interfaces;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
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
        private readonly ILogger<SendIngredientsService> _logger;
        private readonly string _queuename;
        private readonly string _connectionString;

        public SendIngredientsService(IConfiguration configuration, ILogger<SendIngredientsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceBusNamespace = _configuration.GetServiceBusNamespace(); 
            _queuename = _configuration.GetSection("serviceBus:queueName").Value;
            _connectionString = _configuration.GetSection("serviceBus:connectionString").Value;
        }

        public void SendIngredients(LabelImageAdded labelImageAdded)
        {
            try
            {
                string messageBody = JsonConvert.SerializeObject(labelImageAdded);

                _logger.LogInformation("Imagem serializada");

                SendMessagesAsync(messageBody).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Falha ao enviar ingredientes - {ex.Message}");
                throw new Exception("Falha ao enviar ingredientes!");
            }
        }

        private async Task SendMessagesAsync(string messageBody)
        {
            CreateQueueIfNotExists();

            _logger.LogInformation("Conectando na fila");
            var queueClient = new QueueClient(_connectionString, _queuename);

            _logger.LogInformation("Enviando mensagem para fila");
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);

            _logger.LogInformation("Mensagem Enviada");
        }

        private void CreateQueueIfNotExists()
        {
            _logger.LogInformation("Criando fila");

            if (!_serviceBusNamespace.Queues.List().Any(x => x.Name.Equals(_queuename)) &&
                !_serviceBusNamespace.Topics.List().Any(x => x.Name.Equals(_queuename)))
                _serviceBusNamespace.Queues.Define(_queuename).WithSizeInMB(1024).Create();

            _logger.LogInformation("Fila criada");
        }       
    }
}
