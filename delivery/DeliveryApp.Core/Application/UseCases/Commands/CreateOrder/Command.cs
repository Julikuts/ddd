using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    /// <summary>
    /// Создать заказ
    /// </summary>
    public class Command : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор корзины
        /// </summary>
        /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
        public Guid BasketId { get; }

        /// <summary>
        ///     Адрес
        /// </summary>
        public string Address { get; }

        /// <summary>
        ///     Вес
        /// </summary>
        public int Weight { get;  }

        /// <summary>
        /// Ctr
        /// </summary>
        private Command()
        { }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="basketId">Идентификатор корзины</param> 
        /// <param name="address">Адрес</param>
        /// <param name="weight">Вес</param>
        public Command(Guid basketId, string address, int weight)
        {
            //Валидация
            if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException(nameof(address));
            if (weight <= 0) throw new ArgumentException(nameof(weight));

            //Присваивание
            BasketId = basketId;
            Address = address;
            Weight = weight;
        }
    }
}