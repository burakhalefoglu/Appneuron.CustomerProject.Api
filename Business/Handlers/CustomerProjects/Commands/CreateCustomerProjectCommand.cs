using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.ValidationRules;
using Business.MessageBrokers;
using Business.MessageBrokers.Models;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.MessageBrokers;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerProjectCommand : IRequest<IResult>
    {
        public string ProjectName { get; set; }
        public string ProjectBody { get; set; }

        public class CreateCustomerProjectCommandHandler : IRequestHandler<CreateCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMessageBroker _messageBroker;

            public CreateCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository,
                IMessageBroker messageBroker,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerProjectRepository = customerProjectRepository;
                _httpContextAccessor = httpContextAccessor;
                _messageBroker = messageBroker;
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

                var isThereCustomerProjectRecord = await _customerProjectRepository.GetAsync(u =>
                    u.ProjectName == request.ProjectName &&
                    u.CustomerId == Convert.ToInt64(userId) &&
                    u.Status == true);

                if (isThereCustomerProjectRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var projectId = SecurityKeyHelper.GetRandomHexNumber(64).ToLower();
                var addedCustomerProject = new CustomerProject
                {
                    ProjectName = request.ProjectName,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    CustomerId = Convert.ToInt64(userId),
                    ProjectBody = request.ProjectBody
                };

                await _customerProjectRepository.AddAsync(addedCustomerProject);

                var projectModel = new ProjectMessageCommand
                {
                    UserId = userId,
                    ProjectKey = projectId
                };

                await _messageBroker.SendMessageAsync(projectModel);

                return new SuccessDataResult<CustomerProject>(addedCustomerProject, Messages.Added);
            }
        }
    }
}