using Business.Fakes.Handlers.Clients;
using Business.Fakes.Handlers.CustomerProjects;
using Business.MessageBrokers.RabbitMq.Models;
using MassTransit;
using MediatR;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Business.MessageBrokers.RabbitMq.Consumers
{
    public class CreateClientMessageCommandConsumer : IConsumer<CreateClientMessageComamnd>
    {
        private readonly IMediator _mediator;

        public CreateClientMessageCommandConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreateClientMessageComamnd> context)
        {
            var resultProject = await _mediator.Send(new GetCustomerProjectInternalQuery
            {
                ProjectKey = context.Message.ProjectKey
            });

            var result = await _mediator.Send(new CreateClientInternalCommand
            {
                ClientId = context.Message.ClientId,
                ProjectId = resultProject.Data.Id,
                ProjectKey = context.Message.ProjectKey,
                CreatedAt = context.Message.CreatedAt,
                IsPaidClient = context.Message.IsPaidClient
            });

            Debug.WriteLine($"Message: {result.Message} , Success: {result.Success}");
        }
    }
}