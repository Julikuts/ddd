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

        /// <summary>
        /// Ctr
        /// </summary>
        /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
        public OrderRepositoryShould()
        {
            var weightCreateResult = Weight.Create(2);
            weightCreateResult.IsSuccess.Should().BeTrue();
            _weight = weightCreateResult.Value;

            var locationCreateResult = Location.Create(1, 1);
            locationCreateResult.IsSuccess.Should().BeTrue();
            _location = locationCreateResult.Value;
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
            var order = Order.Create(orderId, _location, _weight).Value;

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
            //Arrange
            var courier = Courier.Create("Иван", Transport.Pedestrian).Value;

            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, _location, _weight).Value;

            var orderRepository = new OrderRepository(_context);
            orderRepository.Add(order);
            
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var orderAssignToCourierResult = order.AssignToCourier(courier);
            orderAssignToCourierResult.IsSuccess.Should().BeTrue();
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
            var order = Order.Create(orderId, _location, _weight).Value;

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
            var courier = Courier.Create("Иван", Transport.Pedestrian).Value;

            var order1Id = Guid.NewGuid();
            var order1 = Order.Create(order1Id, _location, _weight).Value;
            var orderAssignToCourierResult = order1.AssignToCourier(courier);
            orderAssignToCourierResult.IsSuccess.Should().BeTrue();

            var order2Id = Guid.NewGuid();
            var order2 = Order.Create(order2Id, _location, _weight).Value;

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