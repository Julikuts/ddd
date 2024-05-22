using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveToOrder
{
    /// <summary>
    /// Передвинуть курьеров
    /// </summary>
    public class Command : IRequest<bool>
    {
        /// <summary>
        /// Ctr
        /// </summary>
        public Command()
        {
        }
    }
}