using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Discounts.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteDiscountCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, IResult>
        {
            private readonly IDiscountRepository _discountRepository;
            private readonly IMediator _mediator;

            public DeleteDiscountCommandHandler(IDiscountRepository discountRepository, IMediator mediator)
            {
                _discountRepository = discountRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
            {
                var discountToDelete = await _discountRepository.GetAsync(p => p.Id == request.Id);
                if (discountToDelete == null)
                { 
                    return new ErrorResult(Messages.DiscountNotFound);
                }
                _discountRepository.Delete(discountToDelete);
                await _discountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}