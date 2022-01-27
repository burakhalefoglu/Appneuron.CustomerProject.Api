using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerScales.Queries
{
    public class GetCustomerScaleQuery : IRequest<IDataResult<CustomerScale>>
    {
        public string Id { get; set; }

        public class GetCustomerScaleQueryHandler : IRequestHandler<GetCustomerScaleQuery, IDataResult<CustomerScale>>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;

            public GetCustomerScaleQueryHandler(ICustomerScaleRepository customerScaleRepository)
            {
                _customerScaleRepository = customerScaleRepository;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerScale>> Handle(GetCustomerScaleQuery request,
                CancellationToken cancellationToken)
            {
                var customerScale = await _customerScaleRepository.GetAsync(p => p.ObjectId == request.Id);
                return new SuccessDataResult<CustomerScale>(customerScale);
            }
        }
    }
}