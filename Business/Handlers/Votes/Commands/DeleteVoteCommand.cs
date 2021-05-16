using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Votes.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteVoteCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteVoteCommandHandler : IRequestHandler<DeleteVoteCommand, IResult>
        {
            private readonly IVoteRepository _voteRepository;
            private readonly IMediator _mediator;

            public DeleteVoteCommandHandler(IVoteRepository voteRepository, IMediator mediator)
            {
                _voteRepository = voteRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
            {
                var voteToDelete = _voteRepository.Get(p => p.Id == request.Id);

                _voteRepository.Delete(voteToDelete);
                await _voteRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}