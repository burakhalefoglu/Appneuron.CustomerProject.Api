using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.GamePlatforms.Queries
{
    public class GetGamePlatformQuery : IRequest<IDataResult<GamePlatform>>
    {
        public string Id { get; set; }

        public class GetGamePlatformQueryHandler : IRequestHandler<GetGamePlatformQuery, IDataResult<GamePlatform>>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;

            public GetGamePlatformQueryHandler(IGamePlatformRepository gamePlatformRepository)
            {
                _gamePlatformRepository = gamePlatformRepository;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<GamePlatform>> Handle(GetGamePlatformQuery request,
                CancellationToken cancellationToken)
            {
                var gamePlatform = await _gamePlatformRepository.GetAsync(p => p.ObjectId == request.Id);
                return new SuccessDataResult<GamePlatform>(gamePlatform);
            }
        }
    }
}