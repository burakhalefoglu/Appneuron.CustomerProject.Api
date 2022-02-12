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
using MediatR;

namespace Business.Handlers.Invoices.Commands
{
    public class UpdateInvoiceCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string BillNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPaymentTime { get; set; }
        public long UserId { get; set; }
        public long DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }

        public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, IResult>
        {
            private readonly IInvoiceRepository _invoiceRepository;

            public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            [ValidationAspect(typeof(UpdateInvoiceValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
            {
                var isThereInvoiceRecord = await _invoiceRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

                if (isThereInvoiceRecord == null) return new ErrorResult(Messages.InvoiceNotFound);

                isThereInvoiceRecord.BillNo = request.BillNo;
                isThereInvoiceRecord.CreatedAt = request.CreatedAt;
                isThereInvoiceRecord.LastPaymentTime = request.LastPaymentTime;
                isThereInvoiceRecord.UserId = request.UserId;
                isThereInvoiceRecord.DiscountId = request.DiscountId;
                isThereInvoiceRecord.UnitPrice = request.UnitPrice;
                isThereInvoiceRecord.IsItPaid = request.IsItPaid;

                await _invoiceRepository.UpdateAsync(isThereInvoiceRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}