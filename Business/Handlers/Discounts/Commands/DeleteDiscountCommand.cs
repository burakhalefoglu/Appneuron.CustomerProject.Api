using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Discounts.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteDiscountCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, IResult>
        {
            private readonly IDiscountRepository _discountRepository;

            public DeleteDiscountCommandHandler(IDiscountRepository discountRepository)
            {
                _discountRepository = discountRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
            {
                var discountToDelete = await _discountRepository.GetAsync(p => p.Id == request.Id);
                if (discountToDelete == null) return new ErrorResult(Messages.DiscountNotFound);
                discountToDelete.Status = false;
                await _discountRepository.UpdateAsync(discountToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}