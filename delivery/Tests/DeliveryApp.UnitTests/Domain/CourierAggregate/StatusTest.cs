using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate
{
    public class CourierStatusShould
    {
        public static IEnumerable<object[]> GetCourierStatuses()
        {
            yield return [Status.NotAvailable, "notavailable"];
            yield return [Status.Ready, "ready"];
            yield return [Status.Busy, "busy"];
        }
        
        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreated()
        {
            //Arrange
            var name = "some-status-name";

            //Act
            var result = Status.Create(name);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(name);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnErrorWhenParamsIsInCorrectOnCreated(string name)
        {
            //Arrange

            //Act
            var result = Status.Create(name);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(GetCourierStatuses))]
        public void ReturnCorrectIdAndName(Status status, string name)
        {
            //Arrange

            //Act

            //Assert
            status.Value.Should().Be(name);
        }

        [Theory]
        [InlineData("notavailable")]
        [InlineData("ready")]
        [InlineData("busy")]
        public void CanBeFoundByName(string name)
        {
            //Arrange

            //Act
            var courierStatus = Status.FromName(name).Value;

            //Assert
            courierStatus.Value.Should().Be(name);
        }
        
        [Fact]
        public void ReturnErrorWhenStatusNotFoundByName()
        {
            //Arrange
            var name = "not-existed-status";

            //Act
            var result = Status.FromName(name);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnListOfStatuses()
        {
            //Arrange

            //Act
            var allStatuses = Status.List();

            //Assert
            allStatuses.Should().NotBeEmpty();
        }
    }
}