
using DeliveryApp.Core.Domain.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Confluent.Kafka;
using DeliveryApp.Core.Domain.OrderAggregate;
using OrderStatusChanged;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace DeliveryApp.Infrastructure.Adapters.RabbitMq;

public sealed class OrderProducer : IBusProducer
{
    
        private readonly ProducerConfig _config;
    private readonly string _topicName = "order.status.changed";

    public OrderProducer(string messageBrokerHost)
    {
       if (string.IsNullOrWhiteSpace(messageBrokerHost)) throw new ArgumentException(nameof(messageBrokerHost));
        _config = new ProducerConfig
        {
            BootstrapServers = messageBrokerHost
        };

    }


    public async Task PublishOrderCreatedDomainEvent(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id.ToString(),
            OrderStatus = OrderStatus.Created
        };
         // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = notification.Id.ToString(),
            Value = JsonConvert.SerializeObject(orderStatusChangedIntegrationEvent)
        };
        
        // Отправляем сообщение в Kafka
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);


    }

    public async Task PublishOrderAssignedDomainEvent(OrderAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id.ToString(),
            OrderStatus = OrderStatus.Assigned
        };
         // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = notification.Id.ToString(),
            Value = JsonConvert.SerializeObject(orderStatusChangedIntegrationEvent)
        };
        
        // Отправляем сообщение в Kafka
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }

    public async Task PublishOrderCompletedDomainEvent(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderStatusChangedIntegrationEvent = new OrderStatusChangedIntegrationEvent()
        {
            OrderId = notification.Order.Id.ToString(),
            OrderStatus = OrderStatus.Completed
        };
         // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = notification.Id.ToString(),
            Value = JsonConvert.SerializeObject(orderStatusChangedIntegrationEvent)
        };
        
        // Отправляем сообщение в Kafka
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }
}