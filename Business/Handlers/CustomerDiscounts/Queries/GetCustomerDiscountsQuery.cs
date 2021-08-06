using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerDiscounts.Queries
{
    public class GetCustomerDiscountsQuery : IRequest<IDataResult<IEnumerable<CustomerDiscount>>>
    {
        public class GetCustomerDiscountsQueryHandler : IRequestHandler<GetCustomerDiscountsQuery, IDataResult<IEnumerable<CustomerDiscount>>>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerDiscountsQueryHandler(ICustomerDiscountRepository customerDiscountRepository, IMediator mediator)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerDiscount>>> Handle(GetCustomerDiscountsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<CustomerDiscount>>(await _customerDiscountRepository.GetListAsync());
            }
        }
    }
}