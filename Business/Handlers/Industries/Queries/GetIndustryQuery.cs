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
        public short Id { get; set; }

        public class GetIndustryQueryHandler : IRequestHandler<GetIndustryQuery, IDataResult<Industry>>
        {
            private readonly IIndustryRepository _industryRepository;
            private readonly IMediator _mediator;

            public GetIndustryQueryHandler(IIndustryRepository industryRepository, IMediator mediator)
            {
                _industryRepository = industryRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Industry>> Handle(GetIndustryQuery request,
                CancellationToken cancellationToken)
            {
                var industry = await _industryRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Industry>(industry);
            }
        }
    }
}