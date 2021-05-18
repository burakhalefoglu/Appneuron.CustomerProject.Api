using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerProjects.Queries
{
    public class GetCustomerProjectsQuery : IRequest<IDataResult<IEnumerable<CustomerProject>>>
    {
        public class GetCustomerProjectsQueryHandler : IRequestHandler<GetCustomerProjectsQuery, IDataResult<IEnumerable<CustomerProject>>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerProjectsQueryHandler(ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerProject>>> Handle(GetCustomerProjectsQuery request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                return new SuccessDataResult<IEnumerable<CustomerProject>>(await _customerProjectRepository.GetListAsync(p => p.CustomerId == userId));
            }
        }
    }
}