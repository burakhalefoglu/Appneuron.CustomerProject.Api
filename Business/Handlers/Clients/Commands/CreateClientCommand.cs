using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.CustomerProjects;
using Business.Handlers.Clients.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Clients.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateClientCommand : IRequest<IResult>
    {
        public string ClientId { get; set; }
        public string ProjectKey { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }

        public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public CreateClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateClientValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateClientCommand request, CancellationToken cancellationToken)
            {
                var resultProject = await _mediator.Send(new GetCustomerProjectInternalQuery
                {
                    ProjectKey = request.ProjectKey
                });

                if (resultProject.Data == null)
                {
                    return new ErrorResult(Messages.ProjectNotFound);
                }

                var resultClient = await _clientRepository.GetAsync(c => c.ClientId == request.ClientId &&
                                                c.ProjectKey == request.ProjectKey);
                if (resultClient != null)
                {
                    return new ErrorResult(Messages.ClientAlreadyExist);
                }

                var addedClient = new Client
                {
                    ClientId = request.ClientId,
                    ProjectId = resultProject.Data.Id,
                    CreatedAt = request.CreatedAt,
                    IsPaidClient = request.IsPaidClient,
                    ProjectKey = request.ProjectKey
                };

                _clientRepository.Add(addedClient);
                await _clientRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}