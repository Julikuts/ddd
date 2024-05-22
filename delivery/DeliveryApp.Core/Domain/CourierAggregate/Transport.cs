using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    /// <summary>
    ///     Статус
    /// </summary>
    public class Transport : Entity<int>
    {
        public static readonly Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1,
            Weight.Create(1).Value);

        public static readonly Transport
            Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(), 2, Weight.Create(4).Value);

        public static readonly Transport
            Scooter = new(3, nameof(Scooter).ToLowerInvariant(), 3, Weight.Create(6).Value);

        public static readonly Transport 
            Car = new(4, nameof(Car).ToLowerInvariant(), 4, Weight.Create(8).Value);

        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(Transport).ToLowerInvariant()}.is.wrong",
                    $"Не верное значение. Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
            }
        }

        /// <summary>
        ///     Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Скорость
        /// </summary>
        public int Speed { get; }

        /// <summary>
        ///     Грузоподъемность
        /// </summary>
        public Weight Capacity { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        private Transport()
        { }

        /// <summary>
        ///     Ctr
        /// </summary>
        private Transport(int id, string name, int speed, Weight capacity)
        {
            Id = id;
            Name = name;
            Speed = speed;
            Capacity = capacity;
        }

        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<Transport> List()
        {
            yield return Pedestrian;
            yield return Bicycle;
            yield return Scooter;
            yield return Car;
        }

        /// <summary>
        /// Получить транспорт по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }

        /// <summary>
        /// Получить транспорт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Транспорт</returns>
        public static Result<Transport, Error> From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state == null) return Errors.StatusIsWrong();
            return state;
        }

        /// <summary>
        /// Может ли данный транспорт перевезти данный вес
        /// </summary>
        /// <param name="weight">Вес</param>
        /// <returns>Результат</returns>
        public Result<bool, Error> CanAllocate(Weight weight)
        {
            return Capacity >= weight;
        }
    }
}