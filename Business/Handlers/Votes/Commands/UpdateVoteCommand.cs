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
using MediatR;

namespace Business.Handlers.Votes.Commands
{
    public class UpdateVoteCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string VoteName { get; set; }
        public short VoteValue { get; set; }

        public class UpdateVoteCommandHandler : IRequestHandler<UpdateVoteCommand, IResult>
        {
            private readonly IMediator _mediator;
            private readonly IVoteRepository _voteRepository;

            public UpdateVoteCommandHandler(IVoteRepository voteRepository, IMediator mediator)
            {
                _voteRepository = voteRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateVoteValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateVoteCommand request, CancellationToken cancellationToken)
            {
                var isThereVoteRecord = await _voteRepository.GetAsync(u => u.Id == request.Id);

                if (isThereVoteRecord == null) return new ErrorResult(Messages.VoteNotFound);

                isThereVoteRecord.VoteName = request.VoteName;
                isThereVoteRecord.VoteValue = request.VoteValue;

                _voteRepository.Update(isThereVoteRecord);
                await _voteRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}