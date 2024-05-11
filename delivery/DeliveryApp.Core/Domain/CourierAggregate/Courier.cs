using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    /// <summary>
    ///     Курьер
    /// </summary>
    public class Courier : Aggregate
    {
        public static class Errors
        {
            public static Error TryStopWorkingWithIncompleteDelivery()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.stop.working.with.incomplete.delivery",
                    "Нельзя прекратить работу, если есть незавершенная доставка");
            }

            public static Error TryStartWorkingWhenAlreadyStarted()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.start.working.when.already.started",
                    "Нельзя начать работу, если ее уже начали ранее");
            }

            public static Error TryAssignOrderWhenNotAvailable()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.assign.order.when.not.available",
                    "Нельзя взять заказ в работу, если курьер не начал рабочий день");
            }
            
            public static Error TryAssignOrderWhenCourierHasAlreadyBusy()
            {
                return new($"{nameof(Courier).ToLowerInvariant()}.try.assign.order.when.courier.has.already.busy",
                    "Нельзя взять заказ в работу, если курьер уже занят");
            }
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Вид транспорта
        /// </summary>
        public Transport Transport { get; private set; }

        /// <summary>
        /// Геопозиция (X,Y)
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Статус курьера
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Courier()
        { }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="transport">Транспорт</param>
        private Courier(string name, Transport transport) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Transport = transport;
            Location = Location.MinLocation;
            Status = Status.NotAvailable;
        }

        /// <summary>
        ///  Factory Method
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="transport">Транспорт</param>
        /// <returns>Результат</returns>
        public static Result<Courier, Error> Create(string name, Transport transport)
        {
            if (string.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport));

            return new Courier(name, transport);
        }

        /// <summary>
        /// Изменить местоположение
        /// </summary>
        /// <param name="targetLocation">Геопозиция</param>
        /// <returns>Результат</returns>
        public Result<object, Error> Move(Location targetLocation)
        {
            if (targetLocation == null) return GeneralErrors.ValueIsRequired(nameof(targetLocation));
            if (Location == targetLocation) return new object();
        
            var cruisingRange = Transport.Speed; //запас хода
        
            var newX = Location.X;
            var newY = Location.Y;

            if (newX != targetLocation.X)
            {
                newX = Math.Min(Location.X + cruisingRange, targetLocation.X);
                var traveledX = targetLocation.X - Location.X; // сколько прошли по X
                cruisingRange -= traveledX;
            }
        
            // если ещё остался запас хода и курьер не в точке Y
            if (newY != targetLocation.Y && cruisingRange > 0)
            {
                newY = Math.Min(Location.Y + cruisingRange, targetLocation.Y);
            }

            var reachedLocation = Location.Create(newX, newY).Value;

            // Если курьер выполнял заказ, то он становится свободным 
            if (Status == Status.Busy && reachedLocation == targetLocation)
            {
                Status = Status.Ready;
            }

            Location = reachedLocation;
            return new object();
        }

        /// <summary>
        /// Начать работать
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> StartWork()
        {
            if (Status == Status.Busy) return Errors.TryStartWorkingWhenAlreadyStarted();
            Status = Status.Ready;
            return new object();
        }

        /// <summary>
        /// Взять работу
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> InWork()
        {
            if (Status == Status.NotAvailable) return Errors.TryAssignOrderWhenNotAvailable();
            if (Status == Status.Busy) return Errors.TryAssignOrderWhenCourierHasAlreadyBusy();
            Status = Status.Busy;
            return new object();
        }

        /// <summary>
        /// Завершить работу
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> CompleteOrder()
        {
            Status = Status.Ready;
            return new object();
        }

        /// <summary>
        /// Закончить работать
        /// </summary>
        /// <returns>Результат</returns>
        public Result<object, Error> StopWork()
        {
            if (Status == Status.Busy) return Errors.TryStopWorkingWithIncompleteDelivery();
            Status = Status.NotAvailable;
            return new object();
        }

        /// <summary>
        /// Рассчитать время до точки
        /// </summary>
        /// <param name="location">Конечное местоположение</param>
        /// <returns>Результат</returns>
        public Result<double, Error> CalculateTimeToLocation(Location location)
        {
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

            var distance = Location.DistanceTo(location).Value;
            var time = (double) distance / Transport.Speed;
            return time;
        }
    }
}