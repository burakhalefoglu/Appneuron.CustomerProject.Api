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
            private readonly IMediator _mediator;

            public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMediator mediator)
            {
                _invoiceRepository = invoiceRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
            {
                var invoiceToDelete = await _invoiceRepository.GetAsync(p => p.Id == request.Id);
                if (invoiceToDelete == null) return new ErrorResult(Messages.InvoiceNotFound);

                _invoiceRepository.Delete(invoiceToDelete);
                await _invoiceRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}