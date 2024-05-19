using System.Reflection;
using Api.Filters;
using Api.Formatters;
using Api.OpenApi;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Core.DomainServices;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Primitives;
using Quartz;
using DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

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

            // Ports & Adapters
            services.AddTransient<ICourierRepository, CourierRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IGeoClient>(x => new Client(geoServiceGrpcHost));
           
            // Domain Services
            services.AddTransient<IDispatchService, DispatchService>();

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

            // HTTP Handlers
            services.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("1.0.0", new OpenApiInfo
                {
                    Title = "Delivery Service",
                    Description = "Отвечает за диспетчеризацию доставки",
                    Contact = new OpenApiContact
                    {
                        Name = "Kirill Vetchinkin",
                        Url = new Uri("https://microarch.ru"),
                        Email = "info@microarch.ru"
                    }
                });
                options.CustomSchemaIds(type => type.FriendlyId(true));
                options.IncludeXmlComments(
                    $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
                options.DocumentFilter<BasePathFilter>("");
                options.OperationFilter<GeneratePathParamsValidationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // gRPC
            services.AddGrpcClient<Client>(options => { options.Address = new Uri(geoServiceGrpcHost); });

            // CRON Jobs
            services.AddQuartz(configure =>
            {
                var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
                var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
                configure
                    .AddJob<AssignOrdersJob>(assignOrdersJobKey)
                    .AddTrigger(
                        trigger => trigger.ForJob(assignOrdersJobKey)
                            .WithSimpleSchedule(
                                schedule => schedule.WithIntervalInSeconds(5)
                                    .RepeatForever()))
                    .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                    .AddTrigger(
                        trigger => trigger.ForJob(moveCouriersJobKey)
                            .WithSimpleSchedule(
                                schedule => schedule.WithIntervalInSeconds(2)
                                    .RepeatForever()));
                       configure.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService();
            // Message Broker
            var sp = services.BuildServiceProvider();
            var mediator = sp.GetService<IMediator>();
            services.AddHostedService<ConsumerService>(x => new ConsumerService(mediator,messageBrokerHost));
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

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; })
                .UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "openapi";
                    options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Delivery Service");
                    options.RoutePrefix = string.Empty;
                    options.SwaggerEndpoint("/openapi-original.json", "Swagger Delivery Service");
                });

            app.UseCors();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
