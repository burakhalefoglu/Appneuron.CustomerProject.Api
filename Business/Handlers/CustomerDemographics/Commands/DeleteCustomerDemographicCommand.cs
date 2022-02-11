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
        public long Id { get; set; }

        public class
            DeleteCustomerDemographicCommandHandler : IRequestHandler<DeleteCustomerDemographicCommand, IResult>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;

            public DeleteCustomerDemographicCommandHandler(ICustomerDemographicRepository customerDemographicRepository)
            {
                _customerDemographicRepository = customerDemographicRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerDemographicCommand request,
                CancellationToken cancellationToken)
            {
                var customerDemographicToDelete =
                    await _customerDemographicRepository.GetAsync(p => p.Id == request.Id);
                if (customerDemographicToDelete == null) return new ErrorResult(Messages.CustomerDemographicNotFound);
                customerDemographicToDelete.Status = false;
                await _customerDemographicRepository.UpdateAsync(customerDemographicToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}