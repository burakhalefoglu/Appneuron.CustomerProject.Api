using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.GamePlatforms.Queries
{
    public class GetGamePlatformsQuery : IRequest<IDataResult<IEnumerable<GamePlatform>>>
    {
        public class
            GetGamePlatformsQueryHandler : IRequestHandler<GetGamePlatformsQuery,
                IDataResult<IEnumerable<GamePlatform>>>
        {
            private readonly IGamePlatformRepository _gamePlatformRepository;
            private readonly IMediator _mediator;

            public GetGamePlatformsQueryHandler(IGamePlatformRepository gamePlatformRepository, IMediator mediator)
            {
                _gamePlatformRepository = gamePlatformRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<GamePlatform>>> Handle(GetGamePlatformsQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<GamePlatform>>(await _gamePlatformRepository.GetListAsync());
            }
        }
    }
}