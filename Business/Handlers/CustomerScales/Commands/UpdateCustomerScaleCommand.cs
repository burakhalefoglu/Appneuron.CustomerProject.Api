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
using MediatR;

namespace Business.Handlers.CustomerScales.Commands
{
    public class UpdateCustomerScaleCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public class UpdateCustomerScaleCommandHandler : IRequestHandler<UpdateCustomerScaleCommand, IResult>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;
            private readonly IMediator _mediator;

            public UpdateCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository,
                IMediator mediator)
            {
                _customerScaleRepository = customerScaleRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateCustomerScaleValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerScaleRecord = await _customerScaleRepository.GetAsync(u => u.Id == request.Id);
                if (isThereCustomerScaleRecord == null) return new ErrorResult(Messages.CustomerScaleNotFound);
                isThereCustomerScaleRecord.Name = request.Name;
                isThereCustomerScaleRecord.Description = request.Description;

                _customerScaleRepository.Update(isThereCustomerScaleRecord);
                await _customerScaleRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}