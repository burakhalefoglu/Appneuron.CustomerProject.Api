using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Queries
{
    public class GetCustomerProjectsQuery : IRequest<IDataResult<IEnumerable<CustomerProject>>>
    {
        public class GetCustomerProjectsQueryHandler : IRequestHandler<GetCustomerProjectsQuery,
            IDataResult<IEnumerable<CustomerProject>>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public GetCustomerProjectsQueryHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerProject>>> Handle(GetCustomerProjectsQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;
                
                return new SuccessDataResult<IEnumerable<CustomerProject>>(
                    _customerProjectRepository.GetListAsync().Result.ToList().Where(p =>
                        p.CustomerId == Convert.ToInt64(userId)));
            }
        }
    }
}