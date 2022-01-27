using System.Collections.Generic;
using System.Linq;
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
    public class GetCustomerProjectLookupQuery : IRequest<IDataResult<IEnumerable<CustomerProject>>>
    {
        public class GetCustomerProjectLookupQueryHandler : IRequestHandler<GetCustomerProjectLookupQuery,
            IDataResult<IEnumerable<CustomerProject>>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerProjectLookupQueryHandler(ICustomerProjectRepository customerProjectRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _customerProjectRepository = customerProjectRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerProject>>> Handle(GetCustomerProjectLookupQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                return new SuccessDataResult<IEnumerable<CustomerProject>>(
                    await _customerProjectRepository.GetListAsync(p => p.CustomerId == userId));
            }
        }
    }
}