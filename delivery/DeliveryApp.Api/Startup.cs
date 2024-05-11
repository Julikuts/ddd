using DeliveryApp.Core.DomainServices;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Api
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            Configuration = configuration;
        }

        /// <summary>
        /// Конфигурация
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Health Checks
            services.AddHealthChecks();

            // Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin(); // Не делайте так в проде!
                    });
            });
            
            // Configuration
            services.Configure<Settings>(options => Configuration.Bind(options));
            var connectionString = Configuration["CONNECTION_STRING"];
            var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
            var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];
            
            // БД 
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(connectionString,
                        npgsqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                        });
                    options.EnableSensitiveDataLogging();
                }
            );
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            
            // Domain Services
            services.AddTransient<IDispatchService, DispatchService>();
            
            // Ports & Adapters
            services.AddTransient<ICourierRepository, CourierRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            
            // MediatR 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());

            // Commands
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.CreateOrder.Command, bool>,
                Core.Application.UseCases.Commands.CreateOrder.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.MoveToOrder.Command, bool>,
                Core.Application.UseCases.Commands.MoveToOrder.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.AssignOrders.Command, bool>,
                Core.Application.UseCases.Commands.AssignOrders.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.StartWork.Command, bool>,
                Core.Application.UseCases.Commands.StartWork.Handler>();
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.StopWork.Command, bool>,
                Core.Application.UseCases.Commands.StopWork.Handler>();

            // Queries
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetActiveOrders.Query,
                Core.Application.UseCases.Queries.GetActiveOrders.Response>>(x
                => new Core.Application.UseCases.Queries.GetActiveOrders.Handler(connectionString));
            services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetAllCouriers.Query,
                Core.Application.UseCases.Queries.GetAllCouriers.Response>>(x
                => new Core.Application.UseCases.Queries.GetAllCouriers.Handler(connectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHealthChecks("/health");
            app.UseRouting();
        }
    }
}