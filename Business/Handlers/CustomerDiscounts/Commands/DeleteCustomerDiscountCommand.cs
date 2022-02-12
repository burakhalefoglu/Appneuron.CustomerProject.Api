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

namespace Business.Handlers.CustomerDiscounts.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerDiscountCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteCustomerDiscountCommandHandler : IRequestHandler<DeleteCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;

            public DeleteCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository)
            {
                _customerDiscountRepository = customerDiscountRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerDiscountCommand request,
                CancellationToken cancellationToken)
            {
                var customerDiscountToDelete =
                    await _customerDiscountRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                if (customerDiscountToDelete == null) return new ErrorResult(Messages.CustomerDiscountNotFound);
                customerDiscountToDelete.Status = false;
                await _customerDiscountRepository.UpdateAsync(customerDiscountToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}