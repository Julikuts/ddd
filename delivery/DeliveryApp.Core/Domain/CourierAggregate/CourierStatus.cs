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
    public class CurierStatus : ValueObject
    {
        public CourierStatusEnum CourierStatusValue { get; protected set; }
        public CurierStatus(CourierStatusEnum status)
        {
            CourierStatusValue = status;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return CourierStatusValue;
        }
    }
}