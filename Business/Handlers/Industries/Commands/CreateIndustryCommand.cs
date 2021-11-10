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
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Industries.Commands
{
    /// <summary>
    /// </summary>
    public class CreateIndustryCommand : IRequest<IResult>
    {
        public string Name { get; set; }

        public class CreateIndustryCommandHandler : IRequestHandler<CreateIndustryCommand, IResult>
        {
            private readonly IIndustryRepository _industryRepository;
            private readonly IMediator _mediator;

            public CreateIndustryCommandHandler(IIndustryRepository industryRepository, IMediator mediator)
            {
                _industryRepository = industryRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateIndustryValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateIndustryCommand request, CancellationToken cancellationToken)
            {
                var isThereIndustryRecord = await _industryRepository.GetAsync(u => u.Name == request.Name);

                if (isThereIndustryRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedIndustry = new Industry
                {
                    Name = request.Name
                };

                _industryRepository.Add(addedIndustry);
                await _industryRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}