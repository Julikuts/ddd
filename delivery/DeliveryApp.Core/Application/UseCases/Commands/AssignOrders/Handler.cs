using DeliveryApp.Core.DomainServices;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrders
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatchService;
        
        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository,
            IDispatchService dispatchService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            // Восстанавливаем аггрегаты
            var order = _orderRepository.GetAllNotAssigned().FirstOrDefault();
            if (order == null) return false;
            var couriers = _courierRepository.GetAllReady().ToList();
            if (!couriers.Any()) return false;

            // Распределяем заказы на курьеров
            var dispatchResult = _dispatchService.Dispatch(order, couriers);
            if (dispatchResult.IsFailure) return false;
            var courier = dispatchResult.Value;

            // Сохраняем
            _courierRepository.Update(courier);
            _orderRepository.Update(order);
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}