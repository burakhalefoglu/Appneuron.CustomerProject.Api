using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.GamePlatforms.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.GamePlatforms.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateGamePlatformCommand : IRequest<IResult>
    {
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }
        public System.Collections.Generic.ICollection<ProjectPlatform> ProjectPlatforms { get; set; }

        public class CreateGamePlatformCommandHandler : IRequestHandler<CreateGamePlatformCommand, IResult>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;
            private readonly IMediator _mediator;

            public CreateGamePlatformCommandHandler(IGamePlatformRepository gamePlatformRepository, IMediator mediator)
            {
                _gamePlatformRepository = gamePlatformRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateGamePlatformValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateGamePlatformCommand request, CancellationToken cancellationToken)
            {
                var isThereGamePlatformRecord = _gamePlatformRepository.Query().Any(u => u.PlatformName == request.PlatformName);

                if (isThereGamePlatformRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedGamePlatform = new GamePlatform
                {
                    PlatformName = request.PlatformName,
                    PlatformDescription = request.PlatformDescription,
                    ProjectPlatforms = request.ProjectPlatforms,
                };

                _gamePlatformRepository.Add(addedGamePlatform);
                await _gamePlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}