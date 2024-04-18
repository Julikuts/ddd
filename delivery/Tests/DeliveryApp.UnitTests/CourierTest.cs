using System;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests
{
    public class CourierTest
    {
        [Fact]
        public void CourierCreatedPedestrian()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            Assert.Equal(courier.Location.X, 1);
            Assert.Equal(courier.Location.Y, 1);

        }
        [Fact]
        public void CourierCreatedBicycle()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Bicycle));
            Assert.Equal(courier.Location.X, 1);
            Assert.Equal(courier.Location.Y, 1);

        }

        [Fact]
        public void CourierMoveTo55Pedestrian()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            courier.Move(new Location(5, 5));
            Assert.Equal(courier.Location.X, 5);
            Assert.Equal(courier.Location.Y, 5);

        }
         [Fact]
        public void CourierMoveTo55Bicycle()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Bicycle));
            courier.Move(new Location(5, 5));
            Assert.Equal(courier.Location.X, 5);
            Assert.Equal(courier.Location.Y, 5);

        }

        [Fact]
        public void CourierMoveTo15()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            courier.Move(new Location(1, 5));
            Assert.Equal(courier.Location.X, 1);
            Assert.Equal(courier.Location.Y, 5);

        }
        [Fact]
        public void CourierMoveTo51()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            courier.Move(new Location(5, 1));
            Assert.Equal(courier.Location.X, 5);
            Assert.Equal(courier.Location.Y, 1);

        }
        public void CourierMoveTo51Move32()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            courier.Move(new Location(5, 1));
            Assert.Equal(courier.Location.X, 5);
            Assert.Equal(courier.Location.Y, 1);
            courier.Move(new Location(3, 2));
            Assert.Equal(courier.Location.X, 3);
            Assert.Equal(courier.Location.Y, 2);

        }

        [Fact]
        public void CourierMoveTo1111()
        {
            var courier = new Courier("", new Transport(1, "", TransportTypeEnum.Pedestrian));
            
            //act
            Action act = () => courier.Move(new Location(11, 11));
            //assert
            Exception exception = Assert.Throws<Exception>(act);
            //The thrown exception can be used for even more detailed assertions.
            Assert.Equal("Значение должно быть от 1 до 10", exception.Message);


        }

    }
}