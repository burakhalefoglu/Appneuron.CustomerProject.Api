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

namespace Business.Handlers.CustomerScales.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteCustomerScaleCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteCustomerScaleCommandHandler : IRequestHandler<DeleteCustomerScaleCommand, IResult>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;
            private readonly IMediator _mediator;

            public DeleteCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository, IMediator mediator)
            {
                _customerScaleRepository = customerScaleRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var customerScaleToDelete = _customerScaleRepository.Get(p => p.Id == request.Id);

                _customerScaleRepository.Delete(customerScaleToDelete);
                await _customerScaleRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}