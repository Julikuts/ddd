using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class CourierShould
    {
        public static IEnumerable<object[]> GetTransports()
        {
            yield return [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(3, 1).Value];
            yield return [Transport.Bicycle, Location.Create(1, 10).Value, Location.Create(1, 3).Value];
            yield return [Transport.Bicycle, Location.Create(10, 1).Value, Location.Create(3, 1).Value];
            yield return [Transport.Bicycle, Location.Create(10, 10).Value, Location.Create(3, 1).Value];
        }

        [Fact]
        public void BeCorrectWhenParamsIsCorrect()
        {
            //Arrange
            var transport = Transport.Pedestrian;

            //Act
            var result = Courier.Create("Ваня", transport);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Name.Should().Be("Ваня");
            result.Value.Transport.Should().Be(transport);
            result.Value.Location.Should().Be(Location.MinLocation);
        }

        [Fact]
        public void ReturnValueIsRequiredErrorWhenNameIsEmpty()
        {
            //Arrange
            var name = "";
            var transport = Transport.Pedestrian;

            //Act
            var result = Courier.Create(name, transport);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(GeneralErrors.ValueIsRequired(nameof(name)));
        }

        [Theory]
        [MemberData(nameof(GetTransports))]
        public void CanMove(Transport transport, Location targetLocation, Location locationAfterMove)
        {
            //Arrange
            var courier = Courier.Create("Ваня", transport).Value;

            //Act
            var result = courier.Move(targetLocation);

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Location.Should().Be(locationAfterMove);
        }

        [Fact]
        public void CantMoveToIncorrectLocation()
        {
            //Arrange
            var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;

            //Act
            var result = courier.Move(null);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(GeneralErrors.ValueIsRequired("targetLocation"));
        }

        [Fact]
        public void CanStartWork()
        {
            //Arrange
            var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;

            //Act
            var result = courier.StartWork();

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Status.Should().Be(Status.Ready);
        }

        [Fact]
        public void CanStopWork()
        {
            //Arrange
            var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;
            courier.StartWork();

            //Act
            var result = courier.StopWork();

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Status.Should().Be(Status.NotAvailable);
        }

        [Fact]
        public void CanCalculateTimeToLocation()
        {
            /*
            Изначальная точка курьера: [1,1]
            Целевая точка: [5,10]
            Количестов шагов, необходимое курьеру: 13 (4 по горизонтали и 9 по вертикали)
            Скорость транспорта (пешехода): 1 шаг в 1 такт
            Время подлета: 13/13 = 13.0 тактов потребуется курьеру, чтобы доставить заказ
            */

            //Arrange
            var location = Location.Create(5, 10).Value;
            var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;

            //Act
            var result = courier.CalculateTimeToLocation(location);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(13);
        }
    }
}