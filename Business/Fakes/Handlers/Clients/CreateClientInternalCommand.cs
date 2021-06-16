
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Business.Fakes.Handlers.Clients
{
    public class CreateClientInternalCommand : IRequest<IResult>
    {

        public string ClientId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectKey { get; set; }
        public System.DateTime CreatedAt { get; set; }
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
                    IsPaidClient = request.IsPaidClient,
                    ProjectKey = request.ProjectKey,

                };

                _clientRepository.Add(addedClient);
                await _clientRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}
