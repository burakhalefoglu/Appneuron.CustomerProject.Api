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

namespace Business.Handlers.CustomerDemographics.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerDemographicCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class
            DeleteCustomerDemographicCommandHandler : IRequestHandler<DeleteCustomerDemographicCommand, IResult>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;
            private readonly IMediator _mediator;

            public DeleteCustomerDemographicCommandHandler(ICustomerDemographicRepository customerDemographicRepository,
                IMediator mediator)
            {
                _customerDemographicRepository = customerDemographicRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerDemographicCommand request,
                CancellationToken cancellationToken)
            {
                var customerDemographicToDelete =
                    await _customerDemographicRepository.GetAsync(p => p.ObjectId == request.Id);
                if (customerDemographicToDelete == null) return new ErrorResult(Messages.CustomerDemographicNotFound);
                customerDemographicToDelete.Status = false;
                await _customerDemographicRepository.UpdateAsync(customerDemographicToDelete,
                    x => x.ObjectId == customerDemographicToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}