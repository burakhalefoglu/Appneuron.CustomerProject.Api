using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.GamePlatforms.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteGamePlatformCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteGamePlatformCommandHandler : IRequestHandler<DeleteGamePlatformCommand, IResult>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;
            private readonly IMediator _mediator;

            public DeleteGamePlatformCommandHandler(IGamePlatformRepository gamePlatformRepository, IMediator mediator)
            {
                _gamePlatformRepository = gamePlatformRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteGamePlatformCommand request, CancellationToken cancellationToken)
            {
                var gamePlatformToDelete = _gamePlatformRepository.Get(p => p.Id == request.Id);

                _gamePlatformRepository.Delete(gamePlatformToDelete);
                await _gamePlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}