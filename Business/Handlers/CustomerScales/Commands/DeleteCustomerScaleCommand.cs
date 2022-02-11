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

namespace Business.Handlers.CustomerScales.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerScaleCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteCustomerScaleCommandHandler : IRequestHandler<DeleteCustomerScaleCommand, IResult>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;

            public DeleteCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository)
            {
                _customerScaleRepository = customerScaleRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var customerScaleToDelete = await _customerScaleRepository.GetAsync(p => p.Id == request.Id);
                if (customerScaleToDelete == null) return new ErrorResult(Messages.CustomerScaleNotFound);
                customerScaleToDelete.Status = false;
                await _customerScaleRepository.UpdateAsync(customerScaleToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}