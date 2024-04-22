// Бизнес-правила CourierStatus:

// CourierStatus - это статус курьера, он состоит только из Value
// CourierStatus содержит 3 значения:
// NotAvailable (недоступен)
// Ready (готов к работе)
// Busy (выполняет заказ, занят)

using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.CourierAggregate
{

    /// <summary>
    /// Бизнес-правила CourierStatus:
    /// </summary>
    public class CourierStatus : Entity<int>
    {
        public static readonly CourierStatus NotAvailable = new(1, nameof(NotAvailable).ToLowerInvariant());
        public static readonly CourierStatus Ready = new(2, nameof(Ready).ToLowerInvariant());
        public static readonly CourierStatus Busy = new(3, nameof(Busy).ToLowerInvariant());

        public string Name { get; }
        protected CourierStatus()
        { }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        protected CourierStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static IEnumerable<CourierStatus> List()
        {
            yield return NotAvailable;
            yield return Ready;
            yield return Busy;
        }
        // protected IEnumerable<IComparable> GetEqualityComponents()
        // {
        //     yield return CourierStatus;
        // }
    }
}