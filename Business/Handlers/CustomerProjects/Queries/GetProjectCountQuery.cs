using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Queries
{
    public class GetProjectCountQuery : IRequest<IDataResult<int>>
    {
        public long Id { get; set; }

        public class GetProjectCountQueryHandler : IRequestHandler<GetProjectCountQuery, IDataResult<int>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public GetProjectCountQueryHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<int>> Handle(GetProjectCountQuery request,
                CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var result =
                    await _customerProjectRepository.GetCountAsync(p => p.CustomerId == userId && p.Id == request.Id);
                return new SuccessDataResult<int>(result);
            }
        }
    }
}