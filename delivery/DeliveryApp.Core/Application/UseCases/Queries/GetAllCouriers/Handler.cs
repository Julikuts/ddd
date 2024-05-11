using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
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
                @"SELECT id, name, location_x, location_y FROM public.couriers;"
                , new { });

            if (result.AsList().Count == 0)
                return null;

            var couriers = new List<Courier>();
            foreach (var item in result)
            {
                couriers.Add(MapToCourier(item));
            }

            return new Response(couriers);
        }

        private Courier MapToCourier(dynamic result)
        {
            var location = new Location {X = result.location_x, Y = result.location_y};
            var courier = new Courier {Id = result.id, Name = result.name, Location = location};
            return courier;
        }
    }
}