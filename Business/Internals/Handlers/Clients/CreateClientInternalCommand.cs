using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.Clients
{
    public class CreateClientInternalCommand : IRequest<IResult>
    {
        public long ClientId { get; set; }
        public long ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }

        public class CreateClientCommandHandler : IRequestHandler<CreateClientInternalCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;

            public CreateClientCommandHandler(IClientRepository clientRepository)
            {
                _clientRepository = clientRepository;
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