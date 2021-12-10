using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using HighLoad.ApiModels;
using HighLoad.Entities;
using HighLoad.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace HighLoad.EventHandlers
{
    public class BookDbEventHandler : BaseEventHandler<Book>
    {
        private readonly IConfiguration _configuration;
        private readonly RedisSender _serviceBusSender;

        public BookDbEventHandler(RedisSender serviceBusSender, IConfiguration configuration, ServiceBusClient serviceBusClient) : base(serviceBusClient)
        {
            _serviceBusSender = serviceBusSender;
            _configuration = configuration;
        }

        protected override string QueueName => "db";
        public override async Task Handle(Book message, CancellationToken cancellationToken)
        {
            using (var dbContext = new DbContext(_configuration.GetConnectionString("Database")))
            {
                var possibleBook = await dbContext.Books.AsNoTracking().FirstOrDefaultAsync(t => t.Id ==  message.Id, cancellationToken);
                if (possibleBook is null)
                    await dbContext.Books.AddAsync(message, cancellationToken);
                else
                    dbContext.Books.Update(message);
                await dbContext.SaveChangesAsync(cancellationToken);
                await _serviceBusSender.SendMessage(message, cancellationToken);
            }
        }
    }

    public class BookRedisEventHandler : BaseEventHandler<Book>
    {
        private readonly IMemoryService _redisService;

        public BookRedisEventHandler(IMemoryService redisService, ServiceBusClient serviceBusClient) : base(serviceBusClient)
        {
            _redisService = redisService;
        }

        protected override string QueueName => "redis";
        public override async Task Handle(Book message, CancellationToken cancellationToken)
        {
            await _redisService.AddAsync(message.Id.ToString(), message);
        }
    }
}