﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Discounts.Queries
{
    public class GetDiscountQuery : IRequest<IDataResult<Discount>>
    {
        public long Id { get; set; }

        public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, IDataResult<Discount>>
        {
            private readonly IDiscountRepository _discountRepository;

            public GetDiscountQueryHandler(IDiscountRepository discountRepository)
            {
                _discountRepository = discountRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Discount>> Handle(GetDiscountQuery request,
                CancellationToken cancellationToken)
            {
                var discount = await _discountRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Discount>(discount);
            }
        }
    }
}