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

namespace Business.Handlers.Votes.Queries
{
    public class GetVotesQuery : IRequest<IDataResult<IEnumerable<Vote>>>
    {
        public class GetVotesQueryHandler : IRequestHandler<GetVotesQuery, IDataResult<IEnumerable<Vote>>>
        {
            private readonly IMediator _mediator;
            private readonly IVoteRepository _voteRepository;

            public GetVotesQueryHandler(IVoteRepository voteRepository, IMediator mediator)
            {
                _voteRepository = voteRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Vote>>> Handle(GetVotesQuery request,
                CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Vote>>(await _voteRepository.GetListAsync(p=> p.Status == true));
            }
        }
    }
}