using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Industries.Queries
{
    public class GetIndustriesQuery : IRequest<IDataResult<IEnumerable<Industry>>>
    {
        public class GetIndustriesQueryHandler : IRequestHandler<GetIndustriesQuery, IDataResult<IEnumerable<Industry>>>
        {
            private readonly IIndustryRepository _industryRepository;
            private readonly IMediator _mediator;

            public GetIndustriesQueryHandler(IIndustryRepository industryRepository, IMediator mediator)
            {
                _industryRepository = industryRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Industry>>> Handle(GetIndustriesQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Industry>>(await _industryRepository.GetListAsync());
            }
        }
    }
}