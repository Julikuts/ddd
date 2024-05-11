using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel
{
    public class WeightShould
    {
        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreated()
        {
            //Arrange

            //Act
            var weight = Weight.Create(10);

            //Assert
            weight.IsSuccess.Should().BeTrue();
            weight.Value.Value.Should().Be(10);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReturnErrorWhenParamsIsInCorrectOnCreated(int kilograms)
        {
            //Arrange

            //Act
            var weight = Weight.Create(kilograms);

            //Assert
            weight.IsSuccess.Should().BeFalse();
            weight.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var first = Weight.Create(10).Value;
            var second = Weight.Create(10).Value;

            //Act
            var result = first == second;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeNotEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var first = Weight.Create(10).Value;
            var second = Weight.Create(5).Value;

            //Act
            var result = first == second;

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanCompareMoreThen()
        {
            //Arrange
            var first = Weight.Create(10).Value;
            var second = Weight.Create(5).Value;

            //Act
            var result = first > second;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanCompareLessThen()
        {
            //Arrange
            var first = Weight.Create(5).Value;
            var second = Weight.Create(10).Value;

            //Act
            var result = first < second;

            //Assert
            result.Should().BeTrue();
        }
    }
}