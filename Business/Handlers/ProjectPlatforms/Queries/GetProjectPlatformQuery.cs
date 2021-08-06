using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ProjectPlatforms.Queries
{
    public class GetProjectPlatformQuery : IRequest<IDataResult<IEnumerable<ProjectPlatform>>>
    {
        public long ProjectId { get; set; }

        public class GetProjectPlatformQueryHandler : IRequestHandler<GetProjectPlatformQuery, IDataResult<IEnumerable<ProjectPlatform>>>
        {
            private readonly IProjectPlatformRepository _projectPlatformRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetProjectPlatformQueryHandler(IProjectPlatformRepository projectPlatformRepository, IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ProjectPlatform>>> Handle(GetProjectPlatformQuery request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountInternalQuery { Id = request.ProjectId });
                if (result.Data <= 0)
                {
                    return new ErrorDataResult<IEnumerable<ProjectPlatform>>(Messages.ProjectNotFound);
                }

                var projectPlatform = await _projectPlatformRepository.GetListAsync(p => p.ProjectId == request.ProjectId);
                return new SuccessDataResult<IEnumerable<ProjectPlatform>>(projectPlatform);
            }
        }
    }
}