using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.AppneuronProducts.Queries
{
    public class GetAppneuronProductsQuery : IRequest<IDataResult<IEnumerable<AppneuronProduct>>>
    {
        public class GetAppneuronProductsQueryHandler : IRequestHandler<GetAppneuronProductsQuery,
            IDataResult<IEnumerable<AppneuronProduct>>>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public GetAppneuronProductsQueryHandler(IAppneuronProductRepository appneuronProductRepository,
                IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<AppneuronProduct>>> Handle(GetAppneuronProductsQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<AppneuronProduct>>(await _appneuronProductRepository
                    .GetListAsync());
            }
        }
    }
}