using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    /// <summary>
    ///     Вес
    /// </summary>
    public class Weight : ValueObject
    {
        /// <summary>
        ///     Значение
        /// </summary>
        public int Value { get; }

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Weight()
        { }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="value">Значение в килограммах</param>
        private Weight(int value)
        {
            Value = value;
        }

        /// <summary>
        ///     Создание
        /// </summary>
        /// <param name="value">Значение в килограммах</param>
        /// <returns>Результат</returns>
        public static Result<Weight, Error> Create(int value)
        {
            if (value <= 0) return GeneralErrors.ValueIsRequired(nameof(value));
            return new Weight(value);
        }

        /// <summary>
        /// Сравнить два веса
        /// </summary>
        /// <param name="first">Вес 1</param>
        /// <param name="second">Вес 2</param>
        /// <returns>Результат</returns>
        public static bool operator <(Weight first, Weight second)
        {
            return first.Value < second.Value;
        }

        /// <summary>
        /// Сравнить два веса
        /// </summary>
        /// <param name="first">Вес 1</param>
        /// <param name="second">Вес 2</param>
        /// <returns>Результат</returns>
        public static bool operator >(Weight first, Weight second)
        {
            return first.Value > second.Value;
        }
        
        /// <summary>
        /// Сравнить два веса
        /// </summary>
        /// <param name="first">Вес 1</param>
        /// <param name="second">Вес 2</param>
        /// <returns>Результат</returns>
        public static bool operator >=(Weight first, Weight second)
        {
            return first.Value >= second.Value;
        }

        /// <summary>
        /// Сравнить два веса
        /// </summary>
        /// <param name="first">Вес 1</param>
        /// <param name="second">Вес 2</param>
        /// <returns>Результат</returns>
        public static bool operator <=(Weight first, Weight second)
        {
            return first.Value <= second.Value;
        }

        /// <summary>
        /// Перегрузка для определения идентичности
        /// </summary>
        /// <returns>Результат</returns>
        /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
        [ExcludeFromCodeCoverage]
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}