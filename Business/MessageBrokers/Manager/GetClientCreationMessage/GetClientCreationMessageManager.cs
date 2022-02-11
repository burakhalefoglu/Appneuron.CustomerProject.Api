using System.Threading.Tasks;
using Business.Internals.Handlers.Clients;
using Business.Internals.Handlers.CustomerProjects;
using Business.MessageBrokers.Models;
using Core.Utilities.Results;
using MediatR;

namespace Business.MessageBrokers.Manager.GetClientCreationMessage
{
    public class GetClientCreationMessageManager : IGetClientCreationMessageService
    {
        private readonly IMediator _mediator;

        public GetClientCreationMessageManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IResult> GetClientCreationMessageQuery(CreateClientMessageComamnd message)
        {
            var resultProject = await _mediator.Send(new GetCustomerProjectInternalQuery
            {
                ProjectId = message.ProjectId
            });

            var result = await _mediator.Send(new CreateClientInternalCommand
            {
                ClientId = message.ClientId,
                ProjectId = resultProject.Data.Id,
                CreatedAt = message.CreatedAt,
                IsPaidClient = message.IsPaidClient
            });

            if (result.Success) return new SuccessResult();

            return new ErrorResult();
        }
    }
}