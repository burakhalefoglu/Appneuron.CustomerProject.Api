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
        public int CustomerId { get; set; }
        public short DiscountId { get; set; }

        public class CreateCustomerDiscountCommandHandler : IRequestHandler<CreateCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IMediator _mediator;

            public CreateCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository,
                IMediator mediator)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCustomerDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
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

                _customerDiscountRepository.Add(addedCustomerDiscount);
                await _customerDiscountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}