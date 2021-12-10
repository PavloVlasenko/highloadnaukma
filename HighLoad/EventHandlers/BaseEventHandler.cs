using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace HighLoad.EventHandlers
{
    public abstract class BaseEventHandler<T> : BackgroundService
    {
        private readonly ServiceBusClient _serviceBusClient;

        protected BaseEventHandler(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        protected abstract string QueueName { get; }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var receiver = _serviceBusClient.CreateReceiver(QueueName);
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                await foreach (var message in receiver.ReceiveMessagesAsync(cancellationToken))
                {
                    try
                    {
                        await Handle(message.Body.ToObjectFromJson<T>(), cancellationToken);
                        await receiver.CompleteMessageAsync(message, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await receiver.DeadLetterMessageAsync(message, cancellationToken: cancellationToken);
                    }
                }
            }
        }

        public abstract Task Handle(T message, CancellationToken cancellationToken);
    }
}