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

namespace Business.Handlers.GamePlatforms.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteGamePlatformCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteGamePlatformCommandHandler : IRequestHandler<DeleteGamePlatformCommand, IResult>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;

            public DeleteGamePlatformCommandHandler(IGamePlatformRepository gamePlatformRepository)
            {
                _gamePlatformRepository = gamePlatformRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteGamePlatformCommand request, CancellationToken cancellationToken)
            {
                var gamePlatformToDelete = await _gamePlatformRepository.GetAsync(p => p.ObjectId == request.Id);

                if (gamePlatformToDelete == null)
                    return new ErrorResult(Messages.GamePlatformNotFound);
                gamePlatformToDelete.Status = false;
                await _gamePlatformRepository.UpdateAsync(gamePlatformToDelete,
                    x => x.ObjectId == gamePlatformToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}