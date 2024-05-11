using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate
{
    /// <summary>
    ///     Статус
    /// </summary>
    public class Status : ValueObject
    {
        public static Status Created => Create(nameof(Created).ToLowerInvariant()).Value;
        public static Status Assigned => Create(nameof(Assigned).ToLowerInvariant()).Value;
        public static Status Completed => Create(nameof(Completed).ToLowerInvariant()).Value;

        /// <summary>
        /// Ошибки, которые может возвращать сущность
        /// </summary>
        public static class Errors
        {
            public static Error StatusIsWrong()
            {
                return new($"{nameof(Status).ToLowerInvariant()}.is.wrong",
                    $"Не верное значение. Допустимые значения: {nameof(Status).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Value))}");
            }
        }

        /// <summary>
        ///     Значение
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Status()
        { }

        /// <summary>
        /// Ctr
        /// </summary>
        private Status(string input)
        {
            Value = input;
        }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <returns>Результат</returns>
        public static Result<Status, Error> Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return GeneralErrors.ValueIsRequired(nameof(input));
            return new Status(input);
        }

        /// <summary>
        /// Получить статус по названию
        /// </summary>
        /// <param name="name">Название</param>
        /// <returns>Статус</returns>
        public static Result<Status, Error> FromName(string name)
        {
            var status = List()
                .SingleOrDefault(s => string.Equals(s.Value, name, StringComparison.CurrentCultureIgnoreCase));
            if (status == null) return Errors.StatusIsWrong();
            return status;
        }

        /// <summary>
        /// Список всех значений списка
        /// </summary>
        /// <returns>Значения списка</returns>
        public static IEnumerable<Status> List()
        {
            yield return Created;
            yield return Assigned;
            yield return Completed;
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