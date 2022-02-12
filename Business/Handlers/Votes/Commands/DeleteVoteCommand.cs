using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Votes.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteVoteCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteVoteCommandHandler : IRequestHandler<DeleteVoteCommand, IResult>
        {
            private readonly IVoteRepository _voteRepository;

            public DeleteVoteCommandHandler(IVoteRepository voteRepository)
            {
                _voteRepository = voteRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
            {
                var voteToDelete = await _voteRepository.GetAsync(p => p.Id == request.Id && p.Status == true);
                if (voteToDelete == null) return new ErrorResult(Messages.VoteNotFound);
                voteToDelete.Status = false;
                await _voteRepository.UpdateAsync(voteToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}