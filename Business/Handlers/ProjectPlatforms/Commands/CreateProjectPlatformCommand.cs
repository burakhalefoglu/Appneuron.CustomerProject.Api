using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Business.Handlers.ProjectPlatforms.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.ProjectPlatforms.Commands
{
    /// <summary>
    /// </summary>
    public class CreateProjectPlatformCommand : IRequest<IResult>
    {
        public long ProjectId { get; set; }
        public short GamePlatformId { get; set; }

        public class CreateProjectPlatformCommandHandler : IRequestHandler<CreateProjectPlatformCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IProjectPlatformRepository _projectPlatformRepository;

            public CreateProjectPlatformCommandHandler(IProjectPlatformRepository projectPlatformRepository,
                IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateProjectPlatformValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateProjectPlatformCommand request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountInternalQuery { Id = request.ProjectId },
                    cancellationToken);
                if (result.Data <= 0) return new ErrorDataResult<ProjectPlatform>(Messages.ProjectNotFound);

                var isThereProjectPlatformRecord =
                    await _projectPlatformRepository.GetAsync(u => u.ProjectId == request.ProjectId);

                if (isThereProjectPlatformRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedProjectPlatform = new ProjectPlatform
                {
                    ProjectId = request.ProjectId,
                    GamePlatformId = request.GamePlatformId
                };

                _projectPlatformRepository.Add(addedProjectPlatform);
                await _projectPlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}