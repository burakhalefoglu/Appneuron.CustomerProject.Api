using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Clients.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteClientCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;

            public DeleteClientCommandHandler(IClientRepository clientRepository)
            {
                _clientRepository = clientRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
            {
                var clientToDelete = await _clientRepository.GetAsync(p => p.ObjectId == request.Id);

                if (clientToDelete == null) return new ErrorResult(Messages.ClientNotFound);
                clientToDelete.Status = false;
                await _clientRepository.UpdateAsync(clientToDelete, x => x.ObjectId == clientToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}