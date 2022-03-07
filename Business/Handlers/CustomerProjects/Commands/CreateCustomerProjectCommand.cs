using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.ValidationRules;
using Business.Handlers.Customers.Commands;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Models;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerProjectCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public class CreateCustomerProjectCommandHandler : IRequestHandler<CreateCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;
            
            public CreateCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerProjectRepository = customerProjectRepository;
                _httpContextAccessor = httpContextAccessor;
                _mediator = mediator;
                
            }

            [ValidationAspect(typeof(CreateCustomerProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [TransactionScopeAspect]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                await _mediator.Send(new CreateCustomerCommand(), cancellationToken);
                
                var isThereCustomerProjectRecord = await _customerProjectRepository.GetAsync(u =>
                    u.Name == request.Name &&
                    u.CustomerId == Convert.ToInt64(userId) &&
                    u.Status == true);

                if (isThereCustomerProjectRecord != null)
                    return new ErrorDataResult<AccessToken>(Messages.NameAlreadyExist);

                var addedCustomerProject = new CustomerProject
                {
                    Name = request.Name,
                    CustomerId = Convert.ToInt64(userId),
                    Description = request.Description
                };

                await _customerProjectRepository.AddAsync(addedCustomerProject);

                return new SuccessResult(Messages.Added);
            }
        }
    }
}