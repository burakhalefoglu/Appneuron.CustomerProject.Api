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
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerDiscounts.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerDiscountCommand : IRequest<IResult>
    {
        public string CustomerId { get; set; }
        public string DiscountId { get; set; }

        public class CreateCustomerDiscountCommandHandler : IRequestHandler<CreateCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;

            public CreateCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository)
            {
                _customerDiscountRepository = customerDiscountRepository;
            }

            [ValidationAspect(typeof(CreateCustomerDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerDiscountCommand request,
                CancellationToken cancellationToken)
            {
                var isThereCustomerDiscountRecord =
                    await _customerDiscountRepository.GetAsync(u => u.UserId == request.CustomerId);

                if (isThereCustomerDiscountRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedCustomerDiscount = new CustomerDiscount
                {
                    UserId = request.CustomerId,
                    DiscountId = request.DiscountId
                };

                await _customerDiscountRepository.AddAsync(addedCustomerDiscount);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}