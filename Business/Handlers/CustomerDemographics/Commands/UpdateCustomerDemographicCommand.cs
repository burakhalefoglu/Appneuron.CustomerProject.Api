using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerDemographics.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerDemographics.Commands
{
    public class UpdateCustomerDemographicCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string CustomerDesc { get; set; }
        public ICollection<Customer> Customers { get; set; }

        public class
            UpdateCustomerDemographicCommandHandler : IRequestHandler<UpdateCustomerDemographicCommand, IResult>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;

            public UpdateCustomerDemographicCommandHandler(ICustomerDemographicRepository customerDemographicRepository)
            {
                _customerDemographicRepository = customerDemographicRepository;
            }

            [ValidationAspect(typeof(UpdateCustomerDemographicValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerDemographicCommand request,
                CancellationToken cancellationToken)
            {
                var isThereCustomerDemographicRecord =
                    await _customerDemographicRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

                if (isThereCustomerDemographicRecord == null)
                    return new ErrorResult(Messages.CustomerDemographicNotFound);

                isThereCustomerDemographicRecord.CustomerDesc = request.CustomerDesc;

                await _customerDemographicRepository.UpdateAsync(isThereCustomerDemographicRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}