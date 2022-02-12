using System.Threading;
using System.Threading.Tasks;
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

namespace Business.Handlers.CustomerDiscounts.Commands
{
    public class UpdateCustomerDiscountCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long DiscountId { get; set; }

        public class UpdateCustomerDiscountCommandHandler : IRequestHandler<UpdateCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;

            public UpdateCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository)
            {
                _customerDiscountRepository = customerDiscountRepository;
            }

            [ValidationAspect(typeof(UpdateCustomerDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerDiscountCommand request,
                CancellationToken cancellationToken)
            {
                var isThereCustomerDiscountRecord =
                    await _customerDiscountRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

                if (isThereCustomerDiscountRecord == null) return new ErrorResult(Messages.CustomerDiscountNotFound);

                isThereCustomerDiscountRecord.UserId = request.CustomerId;
                isThereCustomerDiscountRecord.DiscountId = request.DiscountId;

                await _customerDiscountRepository.UpdateAsync(isThereCustomerDiscountRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}