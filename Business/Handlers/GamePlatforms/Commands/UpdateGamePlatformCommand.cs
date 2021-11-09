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
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.GamePlatforms.Commands
{
    public class UpdateGamePlatformCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }

        public class UpdateGamePlatformCommandHandler : IRequestHandler<UpdateGamePlatformCommand, IResult>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;
            private readonly IMediator _mediator;

            public UpdateGamePlatformCommandHandler(IGamePlatformRepository gamePlatformRepository, IMediator mediator)
            {
                _gamePlatformRepository = gamePlatformRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateGamePlatformValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateGamePlatformCommand request, CancellationToken cancellationToken)
            {
                var isThereGamePlatformRecord = await _gamePlatformRepository.GetAsync(u => u.Id == request.Id);

                if (isThereGamePlatformRecord == null)
                    return new ErrorResult(Messages.GamePlatformNotFound);

                isThereGamePlatformRecord.PlatformName = request.PlatformName;
                isThereGamePlatformRecord.PlatformDescription = request.PlatformDescription;

                _gamePlatformRepository.Update(isThereGamePlatformRecord);
                await _gamePlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}