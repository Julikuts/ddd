using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveToOrder
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            // Восстанавливаем аггрегаты
            var assignedOrders = _orderRepository.GetAllAssigned().ToList();
            if (!assignedOrders.Any())
                return false;

            // Изменяем аггрегаты
            foreach (var order in assignedOrders)
            {
                if (order.CourierId == null)
                    return false;

                var courier = _courierRepository.Get((Guid) order.CourierId);
                if (courier == null) return false;

                var courierMoveResult = courier.Move(order.Location);
                if (courierMoveResult.IsFailure) return false;

                // Если дошли - завершаем заказ, освобождаем курьера
                if (order.Location == courier.Location)
                {
                    order.Complete();
                    courier.CompleteOrder();
                }
                
                _courierRepository.Update(courier);
                _orderRepository.Update(order);
            }

            // Сохраняем
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}