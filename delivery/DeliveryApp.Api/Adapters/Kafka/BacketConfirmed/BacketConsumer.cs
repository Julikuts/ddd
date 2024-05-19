using BasketConfirmed;
using Confluent.Kafka;
using MediatR;
using Newtonsoft.Json;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

public class BacketConsumerService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IConsumer<Ignore, string> _consumer;

    public BacketConsumerService(IMediator mediator, string messageBrokerHost)
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
        _consumer.Subscribe("basket.confirmed");
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
                var basketConfirmedIntegrationEvent = JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);

                //Сервис доставки должен обрабатывать сообщение о том что корзина была оформлена и создавать новый заказ.
                var createOrderCommand = new Core.Application.UseCases.Commands.CreateOrder.Command(
                                                                                                    new Guid(basketConfirmedIntegrationEvent.BasketId),
                                                                                                    basketConfirmedIntegrationEvent.Address,
                                                                                                    basketConfirmedIntegrationEvent.Weight);

                var response = await _mediator.Send(createOrderCommand, cancellationToken);
                if (!response)
                {
                    Console.WriteLine($"Error");
                }


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