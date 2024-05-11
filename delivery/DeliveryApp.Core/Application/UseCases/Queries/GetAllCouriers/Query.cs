using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
{
    public class Query : IRequest<Response>
    {
        public Query()
        {
        }
    }
}