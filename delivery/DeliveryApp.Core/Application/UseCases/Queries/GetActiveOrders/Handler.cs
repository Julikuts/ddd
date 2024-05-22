using Dapper;
using DeliveryApp.Core.Domain.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetActiveOrders
{
    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly string _connectionString;

        public Handler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Response> Handle(Query message, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT id, courier_id, location_x, location_y, weight, status FROM public.orders where status!=@status;"
                , new {status = Status.Completed.Value});

            if (result.AsList().Count == 0)
                return null;

            var orders = new List<Order>();
            foreach (var item in result)
            {
                orders.Add(MapToOrder(item));
            }

            return new Response(orders);
        }

        private Order MapToOrder(dynamic result)
        {
            var location = new Location {X = result.location_x, Y = result.location_y};
            var order = new Order {Id = result.id, Location = location};
            return order;
        }
    }
}