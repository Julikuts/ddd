using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.StopWork
{
    /// <summary>
    /// Начать работу
    /// </summary>
    public class Command : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор курьера
        /// </summary>
        public Guid CourierId { get; private set; }

        /// <summary>
        /// Ctr
        /// </summary>
        private Command()
        { }
        
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="courierId">Идентификатор курьера</param>
        public Command(Guid courierId)
        {
            if (courierId == Guid.Empty) throw new ArgumentException(nameof(courierId));
            CourierId = courierId;
        }
    }
}