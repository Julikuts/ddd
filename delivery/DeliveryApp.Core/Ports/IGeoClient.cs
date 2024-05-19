namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    /// <summary>
    /// Получить информацию о геолокации по адресу
    /// </summary>
    Task<Domain.SharedKernel.Location> GetGeolocationAsync(string address, CancellationToken cancellationToken);
}