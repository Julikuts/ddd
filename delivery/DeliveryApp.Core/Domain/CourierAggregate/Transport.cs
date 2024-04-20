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
        public Transport(int id, string name, TransportTypeEnum transportType)
        {
            Id = id; Name = name;
            TransportType = transportType;

        }


        // Name (название транспорта)
        public string Name { get; protected set; }

        // Speed (скорость)
        public int Speed => 
        TransportType == TransportTypeEnum.Pedestrian ? 1 :
        TransportType == TransportTypeEnum.Bicycle ? 2 :
        TransportType == TransportTypeEnum.Scooter ? 3 :
        TransportType == TransportTypeEnum.Car ? 4 : 0;


        // Capacity (грузоподъемность)

        public Weight Capacity => 
        TransportType == TransportTypeEnum.Pedestrian ? new Weight(1) :
        TransportType == TransportTypeEnum.Bicycle ? new Weight(4) :
        TransportType == TransportTypeEnum.Scooter ? new Weight(6) :
        TransportType == TransportTypeEnum.Car ? new Weight(8) : new Weight(0);


        public TransportTypeEnum TransportType { get; protected set; }

        
        /// <summary>
        /// "может ли данный транспорт перевезти определенный вес?"
        /// </summary>
        public bool CanBeUsed(Weight weight)
        {
            return Capacity.WeightValue <= weight.WeightValue;

        }
    }
}