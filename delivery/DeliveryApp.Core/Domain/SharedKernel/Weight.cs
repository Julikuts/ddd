// Бизнес-правила Weight:

// Weight - это вес, выраженный в килограммах
// Нельзя задать нулевой или отрицательный вес
// Нужно иметь возможность сравнивать Weight с другим
// 2 Weight равны, если их значение в киллограммах равны, обеспечьте функционал проверки на эквивалентность
// Нельзя изменять объект Weight после создания


using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    /// <summary>
    // Weight - это вес, выраженный в килограммах
    /// </summary>
    public class Weight : ValueObject
    {
        public int WeightValue { get; protected set; }
        public Weight(int weight)
        {
            if (weight < 1) throw new Exception("Нельзя задавать отрицательный вес");
            WeightValue = weight;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return WeightValue;
        }
    }
}