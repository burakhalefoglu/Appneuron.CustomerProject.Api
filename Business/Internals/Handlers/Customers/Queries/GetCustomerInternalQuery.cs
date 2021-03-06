using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Internals.Handlers.Customers.Queries;

public class GetCustomerInternalQuery : IRequest<IDataResult<Customer>>
{
    public class GetCustomerInternalQueryHandler : IRequestHandler<GetCustomerInternalQuery, IDataResult<Customer>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCustomerInternalQueryHandler(ICustomerRepository customerRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _customerRepository = customerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [LogAspect(typeof(ConsoleLogger))]
        [SecuredOperation(Priority = 1)]
        public async Task<IDataResult<Customer>> Handle(GetCustomerInternalQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

            var customer = await _customerRepository.GetAsync(p => p.Id == Convert.ToInt64(userId) && p.Status == true);
            return new SuccessDataResult<Customer>(customer);
        }
    }
}