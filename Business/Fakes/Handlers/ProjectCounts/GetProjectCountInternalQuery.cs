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

namespace Business.Fakes.Handlers.ProjectCounts
{
    internal class GetProjectCountInternalQuery : IRequest<IDataResult<int>>
    {
        public long Id { get; set; }

        public class GetProjectCountQueryHandler : IRequestHandler<GetProjectCountInternalQuery, IDataResult<int>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetProjectCountQueryHandler(ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            public async Task<IDataResult<int>> Handle(GetProjectCountInternalQuery request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                int result = await _customerProjectRepository.GetCountAsync(p => p.CustomerId == userId && p.Id == request.Id);
                return new SuccessDataResult<int>(result);
            }
        }
    }
}