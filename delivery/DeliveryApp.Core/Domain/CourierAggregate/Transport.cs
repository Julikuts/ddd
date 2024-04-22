// Бизнес-правила Transport:

// Transport - это транспорт курьера, он состоит из:
// Id (идентификатор)
// Name (название транспорта)
// Speed (скорость)
// Capacity (грузоподъемность)
// Transport бывает 4 видов:
// Pedestrian (пешеход), у которого Speed=1, Capacity=1
// Bicycle (велосипедист), у которого Speed=2, Capacity=4
// Scooter (мопед), у которого Speed=3, Capacity=6
// Car (автомобиль), у которого Speed=4, Capacity=8
// Сущность должна иметь метод, который отвечает на вопрос "может ли данный транспорт перевезти определенный вес?"

using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Transport : Entity<int>
    {
        public Transport(int id, string name)
        {
            Id = id; Name = name;

        }


        // Name (название транспорта)
        public string Name { get; protected set; }

        // Speed (скорость)
        public int Speed =>
        Id == (int)TransportTypeEnum.Pedestrian ? 1 :
        Id == (int)TransportTypeEnum.Bicycle ? 2 :
        Id == (int)TransportTypeEnum.Scooter ? 3 :
        Id == (int)TransportTypeEnum.Car ? 4 : 0;


        // Capacity (грузоподъемность)

        public Weight Capacity =>
        Id == (int) TransportTypeEnum.Pedestrian ? new Weight(1) :
        Id == (int) TransportTypeEnum.Bicycle ? new Weight(4) :
        Id == (int) TransportTypeEnum.Scooter ? new Weight(6) :
        Id == (int) TransportTypeEnum.Car ? new Weight(8) : new Weight(0);


        /// <summary>
        /// "может ли данный транспорт перевезти определенный вес?"
        /// </summary>
        public bool CanBeUsed(Weight weight)
        {
            return Capacity.WeightValue <= weight.WeightValue;

        }

        public static List<Transport> GetAllPossibleTransports()
        {
            var tr = new List<Transport>();
            tr.Add(Pedestrian);
            tr.Add(Bicycle);
            tr.Add(Scooter);
            tr.Add(Car);
            return tr;
        }

        public static Transport Pedestrian =>
            new(1, nameof(TransportTypeEnum.Pedestrian));

        public static Transport Bicycle =>
           new(2, nameof(TransportTypeEnum.Bicycle));

        public static Transport Scooter =>
            new(3, nameof(TransportTypeEnum.Scooter));

        public static Transport Car =>
            new Transport(4, nameof(TransportTypeEnum.Car));
    }
}