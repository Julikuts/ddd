using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetActiveOrders
{
    public class Query : IRequest<Response>
    {
        public Query()
        {
        }
    }
}