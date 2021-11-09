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
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Messaging;

namespace Business.Handlers.CustomerDemographics.Commands
{
    public class UpdateCustomerDemographicCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string CustomerDesc { get; set; }
        public System.Collections.Generic.ICollection<Customer> Customers { get; set; }

        public class UpdateCustomerDemographicCommandHandler : IRequestHandler<UpdateCustomerDemographicCommand, IResult>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;
            private readonly IMediator _mediator;

            public UpdateCustomerDemographicCommandHandler(ICustomerDemographicRepository customerDemographicRepository, IMediator mediator)
            {
                _customerDemographicRepository = customerDemographicRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateCustomerDemographicValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerDemographicCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerDemographicRecord = await _customerDemographicRepository.GetAsync(u => u.Id == request.Id);

                if (isThereCustomerDemographicRecord == null)
                {
                    return new ErrorResult(Messages.CustomerDemographicNotFound);
                }

                isThereCustomerDemographicRecord.CustomerDesc = request.CustomerDesc;
                isThereCustomerDemographicRecord.Customers = request.Customers;

                _customerDemographicRepository.Update(isThereCustomerDemographicRecord);
                await _customerDemographicRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}