using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.GamePlatforms.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.GamePlatforms.Commands
{
    /// <summary>
    /// </summary>
    public class CreateGamePlatformCommand : IRequest<IResult>
    {
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }

        public class CreateGamePlatformCommandHandler : IRequestHandler<CreateGamePlatformCommand, IResult>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;

            public CreateGamePlatformCommandHandler(IGamePlatformRepository gamePlatformRepository)
            {
                _gamePlatformRepository = gamePlatformRepository;
            }

            [ValidationAspect(typeof(CreateGamePlatformValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateGamePlatformCommand request, CancellationToken cancellationToken)
            {
                var isThereGamePlatformRecord =
                    await _gamePlatformRepository.GetAsync(u => u.PlatformName == request.PlatformName);

                if (isThereGamePlatformRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedGamePlatform = new GamePlatform
                {
                    PlatformName = request.PlatformName,
                    PlatformDescription = request.PlatformDescription
                };

                await _gamePlatformRepository.AddAsync(addedGamePlatform);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}