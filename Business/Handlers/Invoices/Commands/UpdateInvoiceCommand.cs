using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Invoices.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Invoices.Commands
{
    public class UpdateInvoiceCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string BillNo { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime LastPaymentTime { get; set; }
        public int UserId { get; set; }
        public short? DiscountId { get; set; }
        public int UnitPrice { get; set; }
        public bool IsItPaid { get; set; }

        public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, IResult>
        {
            private readonly IInvoiceRepository _invoiceRepository;
            private readonly IMediator _mediator;

            public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMediator mediator)
            {
                _invoiceRepository = invoiceRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateInvoiceValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
            {
                var isThereInvoiceRecord = await _invoiceRepository.GetAsync(u => u.Id == request.Id);

                isThereInvoiceRecord.BillNo = request.BillNo;
                isThereInvoiceRecord.CreatedAt = request.CreatedAt;
                isThereInvoiceRecord.LastPaymentTime = request.LastPaymentTime;
                isThereInvoiceRecord.UserId = request.UserId;
                isThereInvoiceRecord.DiscountId = request.DiscountId;
                isThereInvoiceRecord.UnitPrice = request.UnitPrice;
                isThereInvoiceRecord.IsItPaid = request.IsItPaid;

                _invoiceRepository.Update(isThereInvoiceRecord);
                await _invoiceRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}