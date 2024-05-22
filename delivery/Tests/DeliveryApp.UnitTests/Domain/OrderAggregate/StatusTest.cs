using System.Collections.Generic;
using DeliveryApp.Core.Domain.OrderAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.OrderAggregate
{
    public class OrderStatusShould
    {
        public static IEnumerable<object[]> GetOrderStatuses()
        {
            yield return [Status.Created, "created"];
            yield return [Status.Assigned, "assigned"];
            yield return [Status.Completed, "completed"];
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
        [MemberData(nameof(GetOrderStatuses))]
        public void ReturnCorrectIdAndName(Status status, string name)
        {
            //Arrange

            //Act

            //Assert
            status.Value.Should().Be(name);
        }

        [Theory]
        [InlineData("created")]
        [InlineData("assigned")]
        [InlineData("completed")]
        public void CanBeFoundByName(string name)
        {
            //Arrange

            //Act
            var result = Status.FromName(name);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(name);
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