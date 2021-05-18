using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerProjects.Queries
{
    public class GetCustomerProjectQuery : IRequest<IDataResult<CustomerProject>>
    {
        public string ProjectKey { get; set; }

        public class GetCustomerProjectQueryHandler : IRequestHandler<GetCustomerProjectQuery, IDataResult<CustomerProject>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerProjectQueryHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator)
            {
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerProject>> Handle(GetCustomerProjectQuery request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var customerProject = await _customerProjectRepository.GetAsync(p => p.CustomerId == userId && p.ProjectKey == request.ProjectKey);
                return new SuccessDataResult<CustomerProject>(customerProject);
            }
        }
    }
}