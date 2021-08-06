using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Industries.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Industries.Commands
{
    public class UpdateIndustryCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string Name { get; set; }

        public class UpdateIndustryCommandHandler : IRequestHandler<UpdateIndustryCommand, IResult>
        {
            private readonly IIndustryRepository _industryRepository;
            private readonly IMediator _mediator;

            public UpdateIndustryCommandHandler(IIndustryRepository industryRepository, IMediator mediator)
            {
                _industryRepository = industryRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateIndustryValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateIndustryCommand request, CancellationToken cancellationToken)
            {
                var isThereIndustryRecord = await _industryRepository.GetAsync(u => u.Id == request.Id);

                isThereIndustryRecord.Name = request.Name;

                _industryRepository.Update(isThereIndustryRecord);
                await _industryRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}