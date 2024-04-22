using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
public class CourierRepositoryShould: IAsyncLifetime
{
    private ApplicationDbContext _context;
    private Location _location;
    private Weight _weight;
    
    /// <summary>
    /// ????????? Postgres ?? ?????????? TestContainers
    /// </summary>
    /// <remarks>?? ???? ??? Docker ????????? ? Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("order")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();
    
    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>?????????? ???? ??? ????? ????? ??????? ? ?????? ????? ??????</remarks>
    public CourierRepositoryShould()
    {
        var weightCreateResult= new Weight(2);
        _weight = weightCreateResult;
        
        var locationCreateResult = new Location(1,1);
        _location = locationCreateResult;
    }
    
    /// <summary>
    /// ?????????????? ?????????
    /// </summary>
    /// <remarks>?????????? ????? ?????? ??????</remarks>
    public async Task InitializeAsync()
    {
        //???????? ?? (?????????? TestContainers ????????? Docker ????????? ? Postgres)
        await _postgreSqlContainer.StartAsync();
        
        //?????????? ???????? ? ???????????
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_postgreSqlContainer.GetConnectionString(),
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                })
            .Options;
        _context = new ApplicationDbContext(contextOptions);
        _context.Database.Migrate();
    }

    /// <summary>
    /// ?????????? ?????????
    /// </summary>
    /// <remarks>?????????? ????? ??????? ?????</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async void CanAddCourier()
    {
        //Arrange
        var courierCreateResult = new Courier("????", Transport.Pedestrian);
        var courier = courierCreateResult;
        
        //Act
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async void CanUpdateCourier()
    {
        //Arrange
        var courierCreateResult = new Courier("????", Transport.Pedestrian);
        var courier = courierCreateResult;
        
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Act
        courier.CourierStart();
        courierRepository.Update(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
        courierFromDb.CourierStatus.Should().Be(CourierStatusEnum.Ready);
    }
    
    [Fact]
    public async void CanGetById()
    {
        //Arrange
        var courierCreateResult = new Courier("????", Transport.Pedestrian);
        var courier = courierCreateResult;
        
        //Act
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Assert
        var courierFromDb= await courierRepository.GetAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
    }
    
    [Fact]
    public async void CanGetAllActive()
    {
        //Arrange
        var courier1CreateResult = new Courier("????", Transport.Pedestrian);
        var courier1 = courier1CreateResult;
        courier1.CourierStop();
        
        var courier2CreateResult = new Courier("?????", Transport.Pedestrian);
        var courier2 = courier2CreateResult;
        courier2.CourierStart();
        
        var courierRepository = new CourierRepository(_context);
        courierRepository.Add(courier1);
        courierRepository.Add(courier2);
        await courierRepository.UnitOfWork.SaveEntitiesAsync();
        
        //Act
        var activeCouriersFromDb= courierRepository.GetAllReady();
        
        //Assert
        activeCouriersFromDb.Should().NotBeEmpty();
        activeCouriersFromDb.Count().Should().Be(1);
        activeCouriersFromDb.First().Should().BeEquivalentTo(courier2);
        }
    }
}