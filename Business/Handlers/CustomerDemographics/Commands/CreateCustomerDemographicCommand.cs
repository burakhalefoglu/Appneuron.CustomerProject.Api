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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerDemographics.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateCustomerDemographicCommand : IRequest<IResult>
    {
        public string CustomerDesc { get; set; }
        public System.Collections.Generic.ICollection<Customer> Customers { get; set; }

        public class CreateCustomerDemographicCommandHandler : IRequestHandler<CreateCustomerDemographicCommand, IResult>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;
            private readonly IMediator _mediator;

            public CreateCustomerDemographicCommandHandler(ICustomerDemographicRepository customerDemographicRepository, IMediator mediator)
            {
                _customerDemographicRepository = customerDemographicRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCustomerDemographicValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerDemographicCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerDemographicRecord = await  _customerDemographicRepository.GetAsync(u => u.CustomerDesc == request.CustomerDesc);

                if (isThereCustomerDemographicRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedCustomerDemographic = new CustomerDemographic
                {
                    CustomerDesc = request.CustomerDesc,
                    Customers = request.Customers,
                };

                _customerDemographicRepository.Add(addedCustomerDemographic);
                await _customerDemographicRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}