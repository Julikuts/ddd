// Бизнес-правила CourierStatus:

// CourierStatus - это статус курьера, он состоит только из Value
// CourierStatus содержит 3 значения:
// NotAvailable (недоступен)
// Ready (готов к работе)
// Busy (выполняет заказ, занят)
namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public enum CourierStatusEnum
    {
        /// <summary>
        /// недоступен
        /// </summary>
        NotAvailable = 1,
        ///<summary>
        /// готов к работе
        /// </summary>
        Ready = 2,
        ///выполняет заказ, занят <summary>
        /// выполняет заказ, занят
        /// </summary>
        Busy = 3
    }
}