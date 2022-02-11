using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Internals.Handlers.ProjectCounts
{
    public class GetProjectCountInternalQuery : IRequest<IDataResult<int>>
    {
        public long Id { get; set; }

        public class GetProjectCountQueryHandler : IRequestHandler<GetProjectCountInternalQuery, IDataResult<int>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetProjectCountQueryHandler(ICustomerProjectRepository customerProjectRepository)
            {
                _customerProjectRepository = customerProjectRepository;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            public async Task<IDataResult<int>> Handle(GetProjectCountInternalQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var result =
                    await _customerProjectRepository.GetListAsync(p =>
                        p.CustomerId == Convert.ToInt64(userId) && p.Id == request.Id);
                return new SuccessDataResult<int>(result.ToList().Count);
            }
        }
    }
}