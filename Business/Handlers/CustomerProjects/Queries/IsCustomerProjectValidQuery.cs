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

namespace Business.Handlers.CustomerProjects.Queries;

public class IsCustomerProjectValidQuery : IRequest<IDataResult<bool>>
{
    public long ProjectId { get; set; }
    
    public class IsCustomerProjectValidQueryHandler : IRequestHandler<IsCustomerProjectValidQuery,
        IDataResult<bool>>
    {
        private readonly ICustomerProjectRepository _customerProjectRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public IsCustomerProjectValidQueryHandler(ICustomerProjectRepository customerProjectRepository,
            IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _customerProjectRepository = customerProjectRepository;
            _mediator = mediator;
        }

        [PerformanceAspect(5)]
        [LogAspect(typeof(ConsoleLogger))]
        [SecuredOperation(Priority = 1)]
        public async Task<IDataResult<bool>> Handle(IsCustomerProjectValidQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;
                
            return new SuccessDataResult<bool>(
                _customerProjectRepository.GetListAsync().Result.ToList()
                    .Any(p => p.CustomerId == Convert.ToInt64(userId) &&
                              p.Id == request.ProjectId && 
                              p.Status));
        }
    }
}
