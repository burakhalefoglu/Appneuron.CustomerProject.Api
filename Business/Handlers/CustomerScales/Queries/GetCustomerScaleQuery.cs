using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.CustomerScales.Queries
{
    public class GetCustomerScaleQuery : IRequest<IDataResult<CustomerScale>>
    {
        public short Id { get; set; }

        public class GetCustomerScaleQueryHandler : IRequestHandler<GetCustomerScaleQuery, IDataResult<CustomerScale>>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;
            private readonly IMediator _mediator;

            public GetCustomerScaleQueryHandler(ICustomerScaleRepository customerScaleRepository, IMediator mediator)
            {
                _customerScaleRepository = customerScaleRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CustomerScale>> Handle(GetCustomerScaleQuery request, CancellationToken cancellationToken)
            {
                var customerScale = await _customerScaleRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<CustomerScale>(customerScale);
            }
        }
    }
}