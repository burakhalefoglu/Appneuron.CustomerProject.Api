﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Fakes.Handlers.Clients
{
    public class CreateClientInternalCommand : IRequest<IResult>
    {
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }

        public class CreateClientCommandHandler : IRequestHandler<CreateClientInternalCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public CreateClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            public async Task<IResult> Handle(CreateClientInternalCommand request, CancellationToken cancellationToken)
            {
                var addedClient = new Client
                {
                    ClientId = request.ClientId,
                    ProjectId = request.ProjectId,
                    CreatedAt = request.CreatedAt,
                    IsPaidClient = request.IsPaidClient
                };

                await _clientRepository.AddAsync(addedClient);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}