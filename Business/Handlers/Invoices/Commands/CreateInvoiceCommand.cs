using System;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Invoices.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Invoices.Commands
{
    /// <summary>
    /// </summary>
    public class CreateInvoiceCommand : IRequest<IResult>
    {
        public string BillNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public int UserId { get; set; }
        public short? DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }

        public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, IResult>
        {
            private readonly IInvoiceRepository _invoiceRepository;
            private readonly IMediator _mediator;

            public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMediator mediator)
            {
                _invoiceRepository = invoiceRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateInvoiceValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
            {
                var isThereInvoiceRecord = await _invoiceRepository.GetAsync(u => u.BillNo == request.BillNo);

                if (isThereInvoiceRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedInvoice = new Invoice
                {
                    BillNo = request.BillNo,
                    CreatedAt = request.CreatedAt,
                    LastPaymentTime = request.LastPaymentTime,
                    UserId = request.UserId,
                    DiscountId = request.DiscountId,
                    UnitPrice = request.UnitPrice,
                    IsItPaid = request.IsItPaid
                };

                _invoiceRepository.Add(addedInvoice);
                await _invoiceRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}