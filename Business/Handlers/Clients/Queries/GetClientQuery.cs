using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Clients.Queries
{
    public class GetClientQuery : IRequest<IDataResult<Client>>
    {
        public long Id { get; set; }

        public class GetClientQueryHandler : IRequestHandler<GetClientQuery, IDataResult<Client>>
        {
            private readonly IClientRepository _clientRepository;

            public GetClientQueryHandler(IClientRepository clientRepository)
            {
                _clientRepository = clientRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Client>> Handle(GetClientQuery request, CancellationToken cancellationToken)
            {
                var client = await _clientRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Client>(client);
            }
        }
    }
}