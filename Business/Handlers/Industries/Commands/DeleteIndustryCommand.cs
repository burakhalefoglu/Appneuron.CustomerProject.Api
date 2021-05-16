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

namespace Business.Handlers.Industries.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteIndustryCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteIndustryCommandHandler : IRequestHandler<DeleteIndustryCommand, IResult>
        {
            private readonly IIndustryRepository _industryRepository;
            private readonly IMediator _mediator;

            public DeleteIndustryCommandHandler(IIndustryRepository industryRepository, IMediator mediator)
            {
                _industryRepository = industryRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteIndustryCommand request, CancellationToken cancellationToken)
            {
                var industryToDelete = _industryRepository.Get(p => p.Id == request.Id);

                _industryRepository.Delete(industryToDelete);
                await _industryRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}