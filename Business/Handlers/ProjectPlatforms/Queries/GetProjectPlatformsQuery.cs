using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ProjectPlatforms.Queries
{
    public class GetProjectPlatformsQuery : IRequest<IDataResult<IEnumerable<ProjectPlatform>>>
    {
        public class GetProjectPlatformsQueryHandler : IRequestHandler<GetProjectPlatformsQuery, IDataResult<IEnumerable<ProjectPlatform>>>
        {
            private readonly IProjectPlatformRepository _projectPlatformRepository;
            private readonly IMediator _mediator;

            public GetProjectPlatformsQueryHandler(IProjectPlatformRepository projectPlatformRepository, IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ProjectPlatform>>> Handle(GetProjectPlatformsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<ProjectPlatform>>(await _projectPlatformRepository.GetListAsync());
            }
        }
    }
}