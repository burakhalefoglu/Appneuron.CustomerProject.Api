using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Industries.Queries
{
    public class GetIndustryQuery : IRequest<IDataResult<Industry>>
    {
        public long Id { get; set; }

        public class GetIndustryQueryHandler : IRequestHandler<GetIndustryQuery, IDataResult<Industry>>
        {
            private readonly IIndustryRepository _industryRepository;

            public GetIndustryQueryHandler(IIndustryRepository industryRepository)
            {
                _industryRepository = industryRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Industry>> Handle(GetIndustryQuery request,
                CancellationToken cancellationToken)
            {
                var industry = await _industryRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                return new SuccessDataResult<Industry>(industry);
            }
        }
    }
}