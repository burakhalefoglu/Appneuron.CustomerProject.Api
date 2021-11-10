using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Votes.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Votes.Commands
{
    /// <summary>
    /// </summary>
    public class CreateVoteCommand : IRequest<IResult>
    {
        public string VoteName { get; set; }
        public short VoteValue { get; set; }

        public class CreateVoteCommandHandler : IRequestHandler<CreateVoteCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IVoteRepository _voteRepository;

            public CreateVoteCommandHandler(IVoteRepository voteRepository, IMediator mediator)
            {
                _voteRepository = voteRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateVoteValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
            {
                var isThereVoteRecord = await _voteRepository.GetAsync(u => u.VoteName == request.VoteName);

                if (isThereVoteRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedVote = new Vote
                {
                    VoteName = request.VoteName,
                    VoteValue = request.VoteValue
                };

                _voteRepository.Add(addedVote);
                await _voteRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}