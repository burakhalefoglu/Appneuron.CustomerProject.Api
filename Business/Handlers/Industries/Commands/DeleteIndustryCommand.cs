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

namespace Business.Handlers.Industries.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteIndustryCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteIndustryCommandHandler : IRequestHandler<DeleteIndustryCommand, IResult>
        {
            private readonly IIndustryRepository _industryRepository;

            public DeleteIndustryCommandHandler(IIndustryRepository industryRepository)
            {
                _industryRepository = industryRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteIndustryCommand request, CancellationToken cancellationToken)
            {
                var industryToDelete = await _industryRepository.GetAsync(p => p.ObjectId == request.Id);

                if (industryToDelete == null) return new ErrorResult(Messages.IndustryNotFound);
                industryToDelete.Status = false;
                await _industryRepository.UpdateAsync(industryToDelete,
                    x => x.ObjectId == industryToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}