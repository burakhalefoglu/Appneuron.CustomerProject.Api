using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerScales.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.CustomerScales.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerScaleCommand : IRequest<IResult>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public class CreateCustomerScaleCommandHandler : IRequestHandler<CreateCustomerScaleCommand, IResult>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;
            private readonly IMediator _mediator;

            public CreateCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository,
                IMediator mediator)
            {
                _customerScaleRepository = customerScaleRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCustomerScaleValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerScaleRecord = await _customerScaleRepository.GetAsync(u => u.Name == request.Name);

                if (isThereCustomerScaleRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedCustomerScale = new CustomerScale
                {
                    Name = request.Name,
                    Description = request.Description
                };

                _customerScaleRepository.Add(addedCustomerScale);
                await _customerScaleRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}