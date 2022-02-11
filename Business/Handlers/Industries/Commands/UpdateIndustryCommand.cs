using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Industries.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Industries.Commands
{
    public class UpdateIndustryCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public class UpdateIndustryCommandHandler : IRequestHandler<UpdateIndustryCommand, IResult>
        {
            private readonly IIndustryRepository _industryRepository;

            public UpdateIndustryCommandHandler(IIndustryRepository industryRepository)
            {
                _industryRepository = industryRepository;
            }

            [ValidationAspect(typeof(UpdateIndustryValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateIndustryCommand request, CancellationToken cancellationToken)
            {
                var isThereIndustryRecord = await _industryRepository.GetAsync(u => u.Id == request.Id);

                if (isThereIndustryRecord == null) return new ErrorResult(Messages.IndustryNotFound);

                isThereIndustryRecord.Name = request.Name;

                await _industryRepository.UpdateAsync(isThereIndustryRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}