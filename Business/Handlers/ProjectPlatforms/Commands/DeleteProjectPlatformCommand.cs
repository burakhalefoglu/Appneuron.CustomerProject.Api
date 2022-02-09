using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.ProjectPlatforms.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteProjectPlatformCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }

        public class DeleteProjectPlatformCommandHandler : IRequestHandler<DeleteProjectPlatformCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IProjectPlatformRepository _projectPlatformRepository;

            public DeleteProjectPlatformCommandHandler(IProjectPlatformRepository projectPlatformRepository,
                IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteProjectPlatformCommand request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountInternalQuery { Id = request.ProjectId },
                    cancellationToken);
                if (result.Data <= 0) return new ErrorResult(Messages.ProjectNotFound);

                var projectPlatformToDelete = await _projectPlatformRepository.GetAsync(p => p.Id == request.Id);

                if (projectPlatformToDelete == null) return new ErrorResult(Messages.ProjectPlatformNotFound);

                _projectPlatformRepository.Delete(projectPlatformToDelete);
                await _projectPlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}