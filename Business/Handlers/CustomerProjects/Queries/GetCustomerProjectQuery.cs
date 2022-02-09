using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Queries
{
    public class GetCustomerProjectQuery : IRequest<IDataResult<CustomerProject>>
    {
        public string ProjectId { get; set; }

        public class
            GetCustomerProjectQueryHandler : IRequestHandler<GetCustomerProjectQuery, IDataResult<CustomerProject>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public GetCustomerProjectQueryHandler(ICustomerProjectRepository customerProjectRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(ConsoleLogger))]

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerProject>> Handle(GetCustomerProjectQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customerProject = await _customerProjectRepository.GetAsync(p =>
                    p.CustomerId == userId && p.ProjectId == request.ProjectId);
                return new SuccessDataResult<CustomerProject>(customerProject);
            }
        }
    }
}