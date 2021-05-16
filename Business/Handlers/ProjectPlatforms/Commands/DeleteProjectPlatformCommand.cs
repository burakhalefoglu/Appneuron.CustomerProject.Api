using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.Queries;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ProjectPlatforms.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteProjectPlatformCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }

        public class DeleteProjectPlatformCommandHandler : IRequestHandler<DeleteProjectPlatformCommand, IResult>
        {
            private readonly IProjectPlatformRepository _projectPlatformRepository;
            private readonly IMediator _mediator;

            public DeleteProjectPlatformCommandHandler(IProjectPlatformRepository projectPlatformRepository, IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [LoginRequired(Priority = 1)]
            public async Task<IResult> Handle(DeleteProjectPlatformCommand request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountQuery { Id = request.ProjectId });
                if (result.Data <= 0)
                {
                    return new ErrorResult(Messages.ProjectNotFound);
                }

                var projectPlatformToDelete = _projectPlatformRepository.Get(p => p.Id == request.Id);

                _projectPlatformRepository.Delete(projectPlatformToDelete);
                await _projectPlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}