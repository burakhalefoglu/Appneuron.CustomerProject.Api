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

namespace Business.Handlers.CustomerDiscounts.Queries
{
    public class GetCustomerDiscountsQuery : IRequest<IDataResult<IEnumerable<CustomerDiscount>>>
    {
        public class GetCustomerDiscountsQueryHandler : IRequestHandler<GetCustomerDiscountsQuery,
            IDataResult<IEnumerable<CustomerDiscount>>>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public GetCustomerDiscountsQueryHandler(ICustomerDiscountRepository customerDiscountRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerDiscount>>> Handle(GetCustomerDiscountsQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<CustomerDiscount>>(await _customerDiscountRepository
                    .GetListAsync());
            }
        }
    }
}