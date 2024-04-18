using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Xunit;

public class TransportTest
{
    [Theory]
    [InlineData(3)]
    [InlineData(1)]
    public void TransportCanBeUsed(int weightValue)
    {
        var transport = new Transport(1, "test", TransportTypeEnum.Pedestrian);
        Assert.True(transport.CanBeUsed(new Weight(weightValue)));
    }
  }