using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerProjects.Commands
{
    public class UpdateCustomerProjectCommand : IRequest<IResult>
    {
        public string ProjectKey { get; set; }
        public string ProjectName { get; set; }
        public bool Statuse { get; set; }
        public short? VotesId { get; set; }

        public class UpdateCustomerProjectCommandHandler : IRequestHandler<UpdateCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UpdateCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [ValidationAspect(typeof(UpdateCustomerProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [LoginRequired(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var isThereCustomerProjectRecord = await _customerProjectRepository.GetAsync(u => u.ProjectKey == request.ProjectKey && u.CustomerId == userId);
                if (isThereCustomerProjectRecord == null)
                    return new ErrorResult(Messages.UserNotFound);

                isThereCustomerProjectRecord.ProjectName = request.ProjectName;
                isThereCustomerProjectRecord.Statuse = request.Statuse;
                isThereCustomerProjectRecord.VoteId = request.VotesId;

                _customerProjectRepository.Update(isThereCustomerProjectRecord);
                await _customerProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}