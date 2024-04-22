// Бизнес-правила OrderStatus:

// OrderStatus - это статус заказа, он состоит только из Value
// OrderStatus содержит 3 значения:
// Created (создан)
// Assigned (назначен на курьера)
// Completed (завершен)

using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    public class OrderStatus : Entity<int>
    {
        public static readonly OrderStatus Created = new(1, nameof(Created).ToLowerInvariant());
        public static readonly OrderStatus Assigned = new(2, nameof(Assigned).ToLowerInvariant());
        public static readonly OrderStatus Completed = new(3, nameof(Completed).ToLowerInvariant());
    
        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(OrderStatus).ToLowerInvariant()}.is.wrong", 
                    $"Не верное значение. Допустимые значения: {nameof(OrderStatus).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
            }
        }
        
        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        protected OrderStatus()
        {}
    
        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        protected OrderStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<OrderStatus> List()
        {
            yield return Created;
            yield return Assigned;
            yield return Completed;
        }

        // protected override IEnumerable<IComparable> GetEqualityComponents()
        // {
        //    yield return OrderStatusValue;
        // }

   public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            return state;
        }
   public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return state;
        }
    }

}