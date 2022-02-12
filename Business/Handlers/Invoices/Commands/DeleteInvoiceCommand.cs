using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Invoices.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteInvoiceCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, IResult>
        {
            private readonly IInvoiceRepository _invoiceRepository;

            public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
            {
                var invoiceToDelete = await _invoiceRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                if (invoiceToDelete == null) return new ErrorResult(Messages.InvoiceNotFound);
                invoiceToDelete.Status = false;
                await _invoiceRepository.UpdateAsync(invoiceToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}