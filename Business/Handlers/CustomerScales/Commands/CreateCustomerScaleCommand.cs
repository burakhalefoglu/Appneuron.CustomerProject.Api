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

            public CreateCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository)
            {
                _customerScaleRepository = customerScaleRepository;
            }

            [ValidationAspect(typeof(CreateCustomerScaleValidator), Priority = 2)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerScaleRecord = await _customerScaleRepository.GetAsync(u => u.Name == request.Name && u.Status == true);

                if (isThereCustomerScaleRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedCustomerScale = new CustomerScale
                {
                    Name = request.Name,
                    Description = request.Description
                };
                await _customerScaleRepository.AddAsync(addedCustomerScale);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}