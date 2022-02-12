using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Commands
{
    public class UpdateCustomerProjectCommand : IRequest<IResult>
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public bool Statuse { get; set; }
        public long VotesId { get; set; }
        public string ProjectBody { get; set; }

        public class UpdateCustomerProjectCommandHandler : IRequestHandler<UpdateCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public UpdateCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [ValidationAspect(typeof(UpdateCustomerProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var isThereCustomerProjectRecord =
                    await _customerProjectRepository.GetAsync(u =>
                        u.Id == request.ProjectId && u.CustomerId == Convert.ToInt64(userId) && u.Status == true);
                if (isThereCustomerProjectRecord == null)
                    return new ErrorResult(Messages.ProjectNotFound);

                isThereCustomerProjectRecord.ProjectName = request.ProjectName;
                isThereCustomerProjectRecord.Status = request.Statuse;
                isThereCustomerProjectRecord.VoteId = request.VotesId;
                isThereCustomerProjectRecord.ProjectBody = request.ProjectBody;

                await _customerProjectRepository.UpdateAsync(isThereCustomerProjectRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}