using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.AppneuronProducts.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Messaging;

namespace Business.Handlers.AppneuronProducts.Commands
{
    public class UpdateAppneuronProductCommand : IRequest<IResult>
    {
        public short Id { get; set; }
        public string ProductName { get; set; }

        public class UpdateAppneuronProductCommandHandler : IRequestHandler<UpdateAppneuronProductCommand, IResult>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public UpdateAppneuronProductCommandHandler(IAppneuronProductRepository appneuronProductRepository, IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateAppneuronProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateAppneuronProductCommand request, CancellationToken cancellationToken)
            {
                var isThereAppneuronProductRecord = await _appneuronProductRepository.GetAsync(u => u.Id == request.Id);
                if (isThereAppneuronProductRecord == null)
                {
                    return new ErrorResult(Messages.AppneuronProductNotFound);
                }
                isThereAppneuronProductRecord.ProductName = request.ProductName;

                _appneuronProductRepository.Update(isThereAppneuronProductRecord);
                await _appneuronProductRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}