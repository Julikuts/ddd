using DeliveryApp.Core.Ports;
using GeoApp.Ui;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

public class Client:IGeoClient
{
    private readonly string _url;
    private readonly SocketsHttpHandler _socketsHttpHandler;
    private readonly MethodConfig _methodConfig;

    public Client(string url)
    {
        _url = url;
            
        _socketsHttpHandler = new SocketsHttpHandler 
        { 
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan, 
            KeepAlivePingDelay = TimeSpan.FromSeconds(60), 
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true 
        };
        
        _methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }

    public async Task<Core.Domain.SharedKernel.Location> GetGeolocationAsync(string address, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions 
        { 
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
        });
        
        var client = new Geo.GeoClient(channel);
        var reply = await client.GetGeolocationAsync(new GetGeolocationRequest()
        {
            Address = address
        },null, deadline: DateTime.UtcNow.AddSeconds(2),cancellationToken);
            
        var locationCreateRandomResult = Core.Domain.SharedKernel.Location.Create(reply.Location.X, reply.Location.Y);
        if (locationCreateRandomResult.IsFailure) throw new Exception(locationCreateRandomResult.Error.Message); 
        var location = locationCreateRandomResult.Value;
        return location;
    }
}