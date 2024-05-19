using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Api.Adapters.Http
{
    public class DeliveryController : DefaultApiController
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<IActionResult> GetActiveOrders()
        {
            var getActiveOrdersQuery = new Core.Application.UseCases.Queries.GetActiveOrders.Query();
            var response = await _mediator.Send(getActiveOrdersQuery);
            return Ok(response);
        }

        public override async Task<IActionResult> GetAllCouriers()
        {
            var getAllCouriersQuery = new Core.Application.UseCases.Queries.GetAllCouriers.Query();
            var response = await _mediator.Send(getAllCouriersQuery);
            return Ok(response);
        }

        public override async Task<IActionResult> StartWork(Guid courierId)
        {
            var startWorkCommand = new Core.Application.UseCases.Commands.StartWork.Command(courierId);
            var response = await _mediator.Send(startWorkCommand);
            if (response) return Ok();
            return Conflict();
        }

        public override async Task<IActionResult> StopWork(Guid courierId)
        {
            var stopWorkCommand = new Core.Application.UseCases.Commands.StopWork.Command(courierId);
            var response = await _mediator.Send(stopWorkCommand);
            if (response) return Ok();
            return Conflict();
        }
    }
}