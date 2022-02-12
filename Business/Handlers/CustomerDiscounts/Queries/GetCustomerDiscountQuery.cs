using System;
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

namespace Business.Handlers.CustomerDiscounts.Queries
{
    public class GetCustomerDiscountQuery : IRequest<IDataResult<CustomerDiscount>>
    {
        public long DiscountId { get; set; }

        public class
            GetCustomerDiscountQueryHandler : IRequestHandler<GetCustomerDiscountQuery, IDataResult<CustomerDiscount>>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerDiscountQueryHandler(ICustomerDiscountRepository customerDiscountRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerDiscount>> Handle(GetCustomerDiscountQuery request,
                CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customerDiscount =
                    await _customerDiscountRepository.GetAsync(p =>
                        p.DiscountId == request.DiscountId && p.UserId == Convert.ToInt64(userId) && p.Status == true);
                return new SuccessDataResult<CustomerDiscount>(customerDiscount);
            }
        }
    }
}