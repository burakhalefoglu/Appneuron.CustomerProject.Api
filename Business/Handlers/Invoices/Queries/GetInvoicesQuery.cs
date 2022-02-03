﻿using System.Collections.Generic;
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

namespace Business.Handlers.Invoices.Queries
{
    public class GetInvoicesQuery : IRequest<IDataResult<IEnumerable<Invoice>>>
    {
        public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, IDataResult<IEnumerable<Invoice>>>
        {
            private readonly IInvoiceRepository _invoiceRepository;
            private readonly IMediator _mediator;

            public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMediator mediator)
            {
                _invoiceRepository = invoiceRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Invoice>>> Handle(GetInvoicesQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Invoice>>(await _invoiceRepository.GetListAsync());
            }
        }
    }
}