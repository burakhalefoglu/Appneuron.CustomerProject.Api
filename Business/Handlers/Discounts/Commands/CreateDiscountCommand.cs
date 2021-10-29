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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Discounts.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateDiscountCommand : IRequest<IResult>
    {
        public string DiscountName { get; set; }
        public short Percent { get; set; }

        public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, IResult>
        {
            private readonly IDiscountRepository _discountRepository;
            private readonly IMediator _mediator;

            public CreateDiscountCommandHandler(IDiscountRepository discountRepository, IMediator mediator)
            {
                _discountRepository = discountRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
            {
                var isThereDiscountRecord = _discountRepository.Query().Any(u => u.DiscountName == request.DiscountName);

                if (isThereDiscountRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedDiscount = new Discount
                {
                    DiscountName = request.DiscountName,
                    Percent = request.Percent,
                };

                _discountRepository.Add(addedDiscount);
                await _discountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}