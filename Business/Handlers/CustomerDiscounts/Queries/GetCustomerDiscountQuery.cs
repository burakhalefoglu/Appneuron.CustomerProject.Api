using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerDiscounts.Queries
{
    public class GetCustomerDiscountQuery : IRequest<IDataResult<CustomerDiscount>>
    {
        public short DiscountId { get; set; }

        public class GetCustomerDiscountQueryHandler : IRequestHandler<GetCustomerDiscountQuery, IDataResult<CustomerDiscount>>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerDiscountQueryHandler(ICustomerDiscountRepository customerDiscountRepository,
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerDiscount>> Handle(GetCustomerDiscountQuery request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var customerDiscount = await _customerDiscountRepository.GetAsync(p => p.DiscountId == request.DiscountId && p.UserId == userId);
                return new SuccessDataResult<CustomerDiscount>(customerDiscount);
            }
        }
    }
}