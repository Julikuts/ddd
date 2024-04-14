using System;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

namespace DeliveryApp.UnitTests
{
    public class WeightTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WeighValueNotPositive(int weightValue)
        {
            //act
            Action act = () => new Weight(weightValue);
            //assert
            Exception exception = Assert.Throws<Exception>(act);
            //The thrown exception can be used for even more detailed assertions.
            Assert.Equal("Нельзя задавать отрицательный вес", exception.Message);

        }

        [Fact]
        public void WeighValuePositive()
        {
            //act
            Assert.Equal(1, new Weight(1).WeightValue);

        }

        [Fact]
        public void CompareValue()
        {
            var w1 = new Weight(1);
            var w2 = new Weight(1);
            Assert.True(w1.Equals(w2));

        }
    }
}