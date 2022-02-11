using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerDemographics.Queries
{
    public class GetCustomerDemographicQuery : IRequest<IDataResult<CustomerDemographic>>
    {
        public long Id { get; set; }

        public class
            GetCustomerDemographicQueryHandler : IRequestHandler<GetCustomerDemographicQuery,
                IDataResult<CustomerDemographic>>
        {
            private readonly ICustomerDemographicRepository _customerDemographicRepository;

            public GetCustomerDemographicQueryHandler(ICustomerDemographicRepository customerDemographicRepository)
            {
                _customerDemographicRepository = customerDemographicRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerDemographic>> Handle(GetCustomerDemographicQuery request,
                CancellationToken cancellationToken)
            {
                var customerDemographic = await _customerDemographicRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<CustomerDemographic>(customerDemographic);
            }
        }
    }
}