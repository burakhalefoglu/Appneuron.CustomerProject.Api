using Business.BusinessAspects;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Business.Handlers.ProjectPlatforms.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ProjectPlatforms.Commands
{
    public class UpdateProjectPlatformCommand : IRequest<IResult>
    {
        public long ProjectId { get; set; }
        public short GamePlatformId { get; set; }

        public class UpdateProjectPlatformCommandHandler : IRequestHandler<UpdateProjectPlatformCommand, IResult>
        {
            private readonly IProjectPlatformRepository _projectPlatformRepository;
            private readonly IMediator _mediator;

            public UpdateProjectPlatformCommandHandler(IProjectPlatformRepository projectPlatformRepository, IMediator mediator)
            {
                _projectPlatformRepository = projectPlatformRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateProjectPlatformValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateProjectPlatformCommand request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new GetProjectCountInternalQuery { Id = request.ProjectId });
                if (result.Data <= 0)
                {
                    return new ErrorDataResult<ProjectPlatform>(Messages.ProjectNotFound);
                }

                var isThereProjectPlatformRecord = await _projectPlatformRepository.GetAsync(u => u.ProjectId == request.ProjectId);
                isThereProjectPlatformRecord.GamePlatformId = request.GamePlatformId;

                _projectPlatformRepository.Update(isThereProjectPlatformRecord);
                await _projectPlatformRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}