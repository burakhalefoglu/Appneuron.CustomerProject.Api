using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Votes.Queries
{
    public class GetVoteQuery : IRequest<IDataResult<Vote>>
    {
        public short Id { get; set; }

        public class GetVoteQueryHandler : IRequestHandler<GetVoteQuery, IDataResult<Vote>>
        {
            private readonly IVoteRepository _voteRepository;
            private readonly IMediator _mediator;

            public GetVoteQueryHandler(IVoteRepository voteRepository, IMediator mediator)
            {
                _voteRepository = voteRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Vote>> Handle(GetVoteQuery request, CancellationToken cancellationToken)
            {
                var vote = await _voteRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Vote>(vote);
            }
        }
    }
}