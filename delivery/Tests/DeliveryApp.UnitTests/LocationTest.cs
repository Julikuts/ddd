using System;
using DeliveryApp.Core.Domain.SharedKernel;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Helpers;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests
{
    public class LocationTest
    {
        [Fact]
        public void LocationNewXY()
        {
            var loc = new Location(1, 2);
            Assert.Equal(1, loc.X);
            Assert.Equal(2, loc.Y);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        public void LocationNegative(int x)
        {
            //act
            Action act = () => new Location(x, 1);
            //assert
            Exception exception = Assert.Throws<Exception>(act);
            //The thrown exception can be used for even more detailed assertions.
            Assert.Equal("Значение должно быть от 1 до 10", exception.Message);

        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        public void LocationNegativeY(int y)
        {
            //act
            Action act = () => new Location(1, y);
            //assert
            Exception exception = Assert.Throws<Exception>(act);
            //The thrown exception can be used for even more detailed assertions.
            Assert.Equal("Значение должно быть от 1 до 10", exception.Message);

        }

        [Fact]
        public void GetLocationDistinancion()
        {
            var l = new Location(1, 1);
            Assert.Equal(10, l.GetDistanceTo(new Location(6, 6)));

        }

        
 [Fact]
        public void CompareValue()
        {
            var l1 = new Location(2,3);
            var l2 = new Location(2,3);
            Assert.True(l1.Equals(l2));

        }
    }
}