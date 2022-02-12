using System.Threading;
using System.Threading.Tasks;
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

namespace Business.Handlers.AppneuronProducts.Commands
{
    public class UpdateAppneuronProductCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string ProductName { get; set; }

        public class UpdateAppneuronProductCommandHandler : IRequestHandler<UpdateAppneuronProductCommand, IResult>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;

            public UpdateAppneuronProductCommandHandler(IAppneuronProductRepository appneuronProductRepository)
            {
                _appneuronProductRepository = appneuronProductRepository;
            }

            [ValidationAspect(typeof(UpdateAppneuronProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateAppneuronProductCommand request,
                CancellationToken cancellationToken)
            {
                var isThereAppneuronProductRecord =
                    await _appneuronProductRepository.GetAsync(u => u.Id == request.Id && u.Status == true);
                if (isThereAppneuronProductRecord == null) return new ErrorResult(Messages.AppneuronProductNotFound);
                isThereAppneuronProductRecord.ProductName = request.ProductName;

                await _appneuronProductRepository.UpdateAsync(isThereAppneuronProductRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}