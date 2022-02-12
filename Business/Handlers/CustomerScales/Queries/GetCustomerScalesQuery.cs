using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerScales.Queries
{
    public class GetCustomerScalesQuery : IRequest<IDataResult<IEnumerable<CustomerScale>>>
    {
        public class
            GetCustomerScalesQueryHandler : IRequestHandler<GetCustomerScalesQuery,
                IDataResult<IEnumerable<CustomerScale>>>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;
            private readonly IMediator _mediator;

            public GetCustomerScalesQueryHandler(ICustomerScaleRepository customerScaleRepository, IMediator mediator)
            {
                _customerScaleRepository = customerScaleRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CustomerScale>>> Handle(GetCustomerScalesQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<CustomerScale>>(await _customerScaleRepository.GetListAsync(p=> p.Status == true),
                    Messages.DefaultSuccess);
            }
        }
    }
}