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
        public long Id { get; set; }
        public string VoteName { get; set; }
        public short VoteValue { get; set; }

        public class UpdateVoteCommandHandler : IRequestHandler<UpdateVoteCommand, IResult>
        {
            private readonly IVoteRepository _voteRepository;

            public UpdateVoteCommandHandler(IVoteRepository voteRepository)
            {
                _voteRepository = voteRepository;
            }

            [ValidationAspect(typeof(UpdateVoteValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateVoteCommand request, CancellationToken cancellationToken)
            {
                var isThereVoteRecord = await _voteRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

                if (isThereVoteRecord == null) return new ErrorResult(Messages.VoteNotFound);

                isThereVoteRecord.VoteName = request.VoteName;
                isThereVoteRecord.VoteValue = request.VoteValue;

                await _voteRepository.UpdateAsync(isThereVoteRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}