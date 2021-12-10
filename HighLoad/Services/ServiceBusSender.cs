using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace HighLoad.Services
{

    public class DbSender : ServiceBusSender
    {
        public DbSender(IConfiguration configuration) : base(configuration, "db")
        {
        }
    }
    
    public class RedisSender : ServiceBusSender
    {
        public RedisSender(IConfiguration configuration) : base(configuration, "redis")
        {
        }
    }

    public abstract class ServiceBusSender
    {
        private readonly ServiceBusClient _client;
        private readonly Azure.Messaging.ServiceBus.ServiceBusSender _clientSender;

        public ServiceBusSender(IConfiguration configuration, string queueName)
        {
            var connectionString = configuration["ServiceBus"];
            _client = new ServiceBusClient(connectionString);
            _clientSender = _client.CreateSender(queueName);
        }

        public async Task SendMessage<T>(T payload, CancellationToken cancellationToken)
        {
            string messagePayload = JsonSerializer.Serialize(payload);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            await _clientSender.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
        }
    }
}