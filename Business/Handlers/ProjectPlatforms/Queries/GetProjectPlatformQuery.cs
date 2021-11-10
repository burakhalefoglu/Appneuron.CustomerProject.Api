using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.ProjectPlatforms.Queries
{
    public class GetProjectPlatformQuery : IRequest<IDataResult<IEnumerable<ProjectPlatform>>>
    {
        public long ProjectId { get; set; }

        public class GetProjectPlatformQueryHandler : IRequestHandler<GetProjectPlatformQuery,
            IDataResult<IEnumerable<ProjectPlatform>>>
        {
            private readonly IMediator _mediator;
            private readonly IProjectPlatformRepository _projectPlatformRepository;

            public GetProjectPlatformQueryHandler(IProjectPlatformRepository projectPlatformRepository,
                IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ProjectPlatform>>> Handle(GetProjectPlatformQuery request,
                CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountInternalQuery { Id = request.ProjectId },
                    cancellationToken);
                if (result.Data <= 0)
                    return new ErrorDataResult<IEnumerable<ProjectPlatform>>(Messages.ProjectNotFound);

                var projectPlatform =
                    await _projectPlatformRepository.GetListAsync(p => p.ProjectId == request.ProjectId);
                return new SuccessDataResult<IEnumerable<ProjectPlatform>>(projectPlatform);
            }
        }
    }
}