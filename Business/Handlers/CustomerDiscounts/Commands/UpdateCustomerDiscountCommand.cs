using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerDiscounts.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerDiscounts.Commands
{
    public class UpdateCustomerDiscountCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public short DiscountId { get; set; }

        public class UpdateCustomerDiscountCommandHandler : IRequestHandler<UpdateCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IMediator _mediator;

            public UpdateCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository, IMediator mediator)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateCustomerDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerDiscountCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerDiscountRecord = await _customerDiscountRepository.GetAsync(u => u.Id == request.Id);

                isThereCustomerDiscountRecord.UserId = request.CustomerId;
                isThereCustomerDiscountRecord.DiscountId = request.DiscountId;

                _customerDiscountRepository.Update(isThereCustomerDiscountRecord);
                await _customerDiscountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}