using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.StartWork
{
    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// Ctr
        /// </summary>
        public Handler(IUnitOfWork unitOfWork, ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<bool> Handle(Command message, CancellationToken cancellationToken)
        {
            //Восстанавливаем аггрегат
            var courier = _courierRepository.Get(message.CourierId);
            if (courier == null) return false;

            //Изменяем аггрегат
            var courierStartWorkResult = courier.StartWork();
            if (courierStartWorkResult.IsFailure) return false;

            //Сохраняем
            _courierRepository.Update(courier);
            return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}