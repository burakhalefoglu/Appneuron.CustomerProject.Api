using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Discounts.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Discounts.Commands
{
    /// <summary>
    /// </summary>
    public class CreateDiscountCommand : IRequest<IResult>
    {
        public string DiscountName { get; set; }
        public short Percent { get; set; }

        public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, IResult>
        {
            private readonly IDiscountRepository _discountRepository;

            public CreateDiscountCommandHandler(IDiscountRepository discountRepository)
            {
                _discountRepository = discountRepository;
            }

            [ValidationAspect(typeof(CreateDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
            {
                var isThereDiscountRecord =
                    await _discountRepository.GetAsync(u => u.DiscountName == request.DiscountName && u.Status == true);

                if (isThereDiscountRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedDiscount = new Discount
                {
                    DiscountName = request.DiscountName,
                    Percent = request.Percent
                };

                await _discountRepository.AddAsync(addedDiscount);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}