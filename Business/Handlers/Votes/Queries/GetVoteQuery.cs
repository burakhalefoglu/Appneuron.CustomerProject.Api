using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Votes.Queries
{
    public class GetVoteQuery : IRequest<IDataResult<Vote>>
    {
        public long Id { get; set; }

        public class GetVoteQueryHandler : IRequestHandler<GetVoteQuery, IDataResult<Vote>>
        {
            private readonly IVoteRepository _voteRepository;

            public GetVoteQueryHandler(IVoteRepository voteRepository)
            {
                _voteRepository = voteRepository;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Vote>> Handle(GetVoteQuery request, CancellationToken cancellationToken)
            {
                var vote = await _voteRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Vote>(vote);
            }
        }
    }
}