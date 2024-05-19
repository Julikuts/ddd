
using Confluent.Kafka;
using MediatR;
using Newtonsoft.Json;
using OrderStatusChanged;

namespace DeliveryApp.Api.Adapters.Kafka.OrderStatusChanged;

public class OrderConsumerService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IConsumer<Ignore, string> _consumer;
    
    public OrderConsumerService(IMediator mediator, string messageBrokerHost)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        if (string.IsNullOrWhiteSpace(messageBrokerHost)) throw new ArgumentException(nameof(messageBrokerHost));
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = messageBrokerHost,
            GroupId = "DeliveryConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe("order.status.changed");
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
                var orderStatusChangedIntegrationEvent = JsonConvert.DeserializeObject<OrderStatusChangedIntegrationEvent>(consumeResult.Message.Value);
                
                //Тут ваш Use Case
                
                try
                {
                    _consumer.StoreOffset(consumeResult);
                }
                catch (KafkaException e)
                {
                    Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _consumer.Close();
        }
    }
}