﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerScales.Queries
{
    public class GetCustomerScaleQuery : IRequest<IDataResult<CustomerScale>>
    {
        public long Id { get; set; }

        public class GetCustomerScaleQueryHandler : IRequestHandler<GetCustomerScaleQuery, IDataResult<CustomerScale>>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;

            public GetCustomerScaleQueryHandler(ICustomerScaleRepository customerScaleRepository)
            {
                _customerScaleRepository = customerScaleRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerScale>> Handle(GetCustomerScaleQuery request,
                CancellationToken cancellationToken)
            {
                var customerScale = await _customerScaleRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                return new SuccessDataResult<CustomerScale>(customerScale);
            }
        }
    }
}