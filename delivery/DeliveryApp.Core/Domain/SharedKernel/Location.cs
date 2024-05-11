using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    /// <summary>
    /// Координата
    /// </summary>
    public class Location : ValueObject
    {
        /// <summary>
        /// Макимально возможная координата
        /// </summary>
        public static readonly Location MinLocation = new Location(1, 1);

        /// <summary>
        /// Макимально возможная координата
        /// </summary>
        public static readonly Location MaxLocation = new Location(10, 10);

        /// <summary>
        /// Горизонталь
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Вертикаль
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Location()
        { }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="x">Горизонталь</param>
        /// <param name="y">Вертикаль</param>
        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="x">Горизонталь</param>
        /// <param name="y">Вертикаль</param>
        /// <returns>Результат</returns>
        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < MinLocation.X || x > MaxLocation.X) return GeneralErrors.ValueIsInvalid(nameof(x));
            if (y < MinLocation.Y || y > MaxLocation.Y) return GeneralErrors.ValueIsInvalid(nameof(y));

            return new Location(x, y);
        }

        /// <summary>
        /// Создать рандомную координату
        /// </summary>
        /// <returns>Результат</returns>
        public static Result<Location, Error> CreateRandom()
        {
            var rnd = new Random();
            var x = rnd.Next(MinLocation.X, MaxLocation.X);
            var y = rnd.Next(MinLocation.X, MaxLocation.Y);
            var location = new Location(x, y);
            return location;
        }

        /// <summary>
        /// Рассчитать дистанцию
        /// </summary>
        /// <param name="targetLocation">Конечная координата</param>
        /// <returns>Результат</returns>
        public Result<int, Error> DistanceTo(Location targetLocation)
        {
            // Считаем разницу
            var diffX = Math.Abs(X - targetLocation.X);
            var diffY = Math.Abs(Y - targetLocation.Y);
                
            // Считаем дистанцию
            var distance = diffX + diffY; 
            return distance;
        }

        /// <summary>
        /// Перегрузка для определения идентичности
        /// </summary>
        /// <returns>Результат</returns>
        /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
        [ExcludeFromCodeCoverage]
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}