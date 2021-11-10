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
using MediatR;

namespace Business.Handlers.Discounts.Commands
{
    public class UpdateDiscountCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string DiscountName { get; set; }
        public short Percent { get; set; }

        public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, IResult>
        {
            private readonly IDiscountRepository _discountRepository;
            private readonly IMediator _mediator;

            public UpdateDiscountCommandHandler(IDiscountRepository discountRepository, IMediator mediator)
            {
                _discountRepository = discountRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDiscountValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
            {
                var isThereDiscountRecord = await _discountRepository.GetAsync(u => u.Id == request.Id);
                if (isThereDiscountRecord == null) return new ErrorResult(Messages.DiscountNotFound);

                isThereDiscountRecord.DiscountName = request.DiscountName;
                isThereDiscountRecord.Percent = request.Percent;

                _discountRepository.Update(isThereDiscountRecord);
                await _discountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}