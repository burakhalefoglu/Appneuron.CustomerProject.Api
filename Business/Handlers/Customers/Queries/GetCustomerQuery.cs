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

namespace Business.Handlers.Customers.Queries
{
    public class GetCustomerQuery : IRequest<IDataResult<Customer>>
    {
        public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, IDataResult<Customer>>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerQueryHandler(ICustomerRepository customerRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerRepository = customerRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Customer>> Handle(GetCustomerQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customer = await _customerRepository.GetAsync(p => p.ObjectId == userId);
                return new SuccessDataResult<Customer>(customer);
            }
        }
    }
}