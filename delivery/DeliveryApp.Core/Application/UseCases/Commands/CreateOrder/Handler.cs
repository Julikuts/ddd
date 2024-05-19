using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IGeoClient _geoClient;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoClient geoClient)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
             _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            //Получаем геопозицию из Geo (пока ставим фэйковое значение)
            //Получаем геопозицию из Geo (пока ставим фэйковое значение)
            var location = await _geoClient.GetGeolocationAsync(message.Address,cancellationToken);
            
            //Создаем вес
            var weightCreateResult = Weight.Create(message.Weight);
            if (weightCreateResult.IsFailure) return false;
            var weight = weightCreateResult.Value;

            //Создаем заказ
            var orderCreateResult = Order.Create(message.BasketId, location, weight);
            if (orderCreateResult.IsFailure) return false;
            var order = orderCreateResult.Value;

            // Сохраняем
            _orderRepository.Add(order);
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}