using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class OrderRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private Location _location;
        private Weight _weight;

        /// <summary>
        /// Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("order")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();
    public OrderRepositoryShould()
    {
        var weightCreateResult= new Weight(2);
        _weight = weightCreateResult;
        
        var locationCreateResult = new Location(1,1);
        _location = locationCreateResult;
    }
        /// <summary>
        /// Инициализируем окружение
        /// </summary>
        /// <remarks>Вызывается перед каждым тестом</remarks>
        public async Task InitializeAsync()
        {
            //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
            await _postgreSqlContainer.StartAsync();

            //Накатываем миграции и справочники
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                }).Options;
            _context = new ApplicationDbContext(contextOptions);
            _context.Database.Migrate();
        }

        /// <summary>
        /// Уничтожаем окружение
        /// </summary>
        /// <remarks>Вызывается после каждого теста</remarks>
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async void CanAddOrder()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, _location, _weight);

            //Act
            var orderRepository = new OrderRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            
            orderRepository.Add(order);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var orderFromDb = orderRepository.Get(order.Id);
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async void CanUpdateOrder()
        {
             var orderId = Guid.NewGuid();
            //Arrange
        var courier = new Courier("????", Transport.Pedestrian);

           
            var order = new Order(orderId, _location, _weight);

            var orderRepository = new OrderRepository(_context);
            orderRepository.Add(order);
            
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orderAssignToCourierResult = order.Assign(courier);
            orderRepository.Update(order);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var orderFromDb = orderRepository.Get(order.Id);
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async void CanGetById()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, _location, _weight);

            //Act
            var orderRepository = new OrderRepository(_context);
            orderRepository.Add(order);
            
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var orderFromDb = orderRepository.Get(order.Id);
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async void CanGetAllActive()
        {
            //Arrange
            var courier = new Courier("Иван", Transport.Pedestrian);

            var order1Id = Guid.NewGuid();
            var order1 = new Order(order1Id, _location, _weight);
            var orderAssignToCourierResult = order1.Assign(courier);

            var order2Id = Guid.NewGuid();
            var order2 =  new Order(order2Id, _location, _weight);

            var orderRepository = new OrderRepository(_context);
            orderRepository.Add(order1);
            orderRepository.Add(order2);
            
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var activeOrdersFromDb = orderRepository.GetAllNotAssigned();

            //Assert
            var ordersFromDb = activeOrdersFromDb.ToList();
            ordersFromDb.Should().NotBeEmpty();
            ordersFromDb.Count().Should().Be(1);
            ordersFromDb.First().Should().BeEquivalentTo(order2);
        }
    }
}