using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _context;

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
        public CourierRepositoryShould()
        {
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
        public async void CanAddCourier()
        {
            //Arrange
            var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
            courierCreateResult.IsSuccess.Should().BeTrue();
            var courier = courierCreateResult.Value;

            //Act
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            courierRepository.Add(courier);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var courierFromDb = courierRepository.Get(courier.Id);
            courier.Should().BeEquivalentTo(courierFromDb);
        }

        [Fact]
        public async void CanUpdateCourier()
        {
            //Arrange
            var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
            courierCreateResult.IsSuccess.Should().BeTrue();
            var courier = courierCreateResult.Value;

            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            courierRepository.Add(courier);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var courierStartWorkResult = courier.StartWork();
            courierStartWorkResult.IsSuccess.Should().BeTrue();
            courierRepository.Update(courier);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var courierFromDb = courierRepository.Get(courier.Id);
            courier.Should().BeEquivalentTo(courierFromDb);
            courierFromDb.Status.Should().Be(Status.Ready);
        }

        [Fact]
        public async void CanGetById()
        {
            //Arrange
            var courierCreateResult = Courier.Create("Иван", Transport.Pedestrian);
            courierCreateResult.IsSuccess.Should().BeTrue();
            var courier = courierCreateResult.Value;

            //Act
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            courierRepository.Add(courier);
            await unitOfWork.SaveEntitiesAsync();

            //Assert
            var courierFromDb = courierRepository.Get(courier.Id);
            courier.Should().BeEquivalentTo(courierFromDb);
        }

        [Fact]
        public async void CanGetAllActive()
        {
            //Arrange
            var courier1CreateResult = Courier.Create("Иван", Transport.Pedestrian);
            courier1CreateResult.IsSuccess.Should().BeTrue();
            var courier1 = courier1CreateResult.Value;
            courier1.StopWork();

            var courier2CreateResult = Courier.Create("Борис", Transport.Pedestrian);
            courier2CreateResult.IsSuccess.Should().BeTrue();
            var courier2 = courier2CreateResult.Value;
            courier2.StartWork();

            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            courierRepository.Add(courier1);
            courierRepository.Add(courier2);
            await unitOfWork.SaveEntitiesAsync();

            //Act
            var activeCouriersFromDb = courierRepository.GetAllReady();

            //Assert
            var couriersFromDb = activeCouriersFromDb.ToList();
            couriersFromDb.Should().NotBeEmpty();
            couriersFromDb.Count().Should().Be(1);
            couriersFromDb.First().Should().BeEquivalentTo(courier2);
        }
    }
}