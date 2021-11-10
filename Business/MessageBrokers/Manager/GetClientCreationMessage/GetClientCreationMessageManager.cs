using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.Fakes.Handlers.Clients;
using Business.Fakes.Handlers.CustomerProjects;
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
                ProjectKey = message.ProjectKey
            });

            var result = await _mediator.Send(new CreateClientInternalCommand
            {
                ClientId = message.ClientId,
                ProjectId = resultProject.Data.Id,
                ProjectKey = message.ProjectKey,
                CreatedAt = message.CreatedAt,
                IsPaidClient = message.IsPaidClient
            });

            if (result.Success)
            {
                return new SuccessResult();
            }

            return new ErrorResult();
        }
    }
}
