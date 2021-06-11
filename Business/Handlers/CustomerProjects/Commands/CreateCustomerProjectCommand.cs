using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.MessageBrokers.RabbitMq;
using Core.Utilities.MessageBrokers.RabbitMq.Models;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateCustomerProjectCommand : IRequest<IResult>
    {
        public string ProjectName { get; set; }
        public string ProjectBody { get; set; }

        public class CreateCustomerProjectCommandHandler : IRequestHandler<CreateCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMessageBrokerHelper _messageBrockerHelper;
            public CreateCustomerProjectCommandHandler(IMessageBrokerHelper messageBrockerHelper, ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _messageBrockerHelper = messageBrockerHelper;
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [ValidationAspect(typeof(CreateCustomerProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [TransactionScopeAspectAsync]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var isThereCustomerProjectRecord = _customerProjectRepository.Query().Any(u => u.ProjectName == request.ProjectName);

                if (isThereCustomerProjectRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var projectKey = SecurityKeyHelper.GetRandomHexNumber(64).ToLower();
                var addedCustomerProject = new CustomerProject
                {
                    ProjectKey = projectKey,
                    ProjectName = request.ProjectName,
                    Statuse = true,
                    CreatedAt = DateTime.Now,
                    CustomerId = userId,
                    ProjectBody = request.ProjectBody,
                };

                _customerProjectRepository.Add(addedCustomerProject);
                await _customerProjectRepository.SaveChangesAsync();
                var projectModel = new ProjectModel
                {
                    UserId = userId,
                    ProjectKey = projectKey
                };
                _messageBrockerHelper.QueueMessage(projectModel);

                return new SuccessDataResult<CustomerProject>(addedCustomerProject, Messages.Added);
            }
        }
    }
}