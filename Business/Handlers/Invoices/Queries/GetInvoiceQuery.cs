using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Invoices.Queries
{
    public class GetInvoiceQuery : IRequest<IDataResult<Invoice>>
    {
        public long Id { get; set; }

        public class GetInvoiceQueryHandler : IRequestHandler<GetInvoiceQuery, IDataResult<Invoice>>
        {
            private readonly IInvoiceRepository _invoiceRepository;

            public GetInvoiceQueryHandler(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Invoice>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
            {
                var invoice = await _invoiceRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                return new SuccessDataResult<Invoice>(invoice);
            }
        }
    }
}