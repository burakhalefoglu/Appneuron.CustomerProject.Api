using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Discounts.Queries
{
    public class GetDiscountQuery : IRequest<IDataResult<Discount>>
    {
        public short Id { get; set; }

        public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, IDataResult<Discount>>
        {
            private readonly IDiscountRepository _discountRepository;
            private readonly IMediator _mediator;

            public GetDiscountQueryHandler(IDiscountRepository discountRepository, IMediator mediator)
            {
                _discountRepository = discountRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Discount>> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
            {
                var discount = await _discountRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Discount>(discount);
            }
        }
    }
}