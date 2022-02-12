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
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.AppneuronProducts.Commands
{
    /// <summary>
    /// </summary>
    public class CreateAppneuronProductCommand : IRequest<IResult>
    {
        public string ProductName { get; set; }

        public class CreateAppneuronProductCommandHandler : IRequestHandler<CreateAppneuronProductCommand, IResult>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;

            public CreateAppneuronProductCommandHandler(IAppneuronProductRepository appneuronProductRepository)
            {
                _appneuronProductRepository = appneuronProductRepository;
            }

            [ValidationAspect(typeof(CreateAppneuronProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateAppneuronProductCommand request,
                CancellationToken cancellationToken)
            {
                var isThereAppneuronProductRecord =
                    await _appneuronProductRepository.GetAsync(u => u.ProductName == request.ProductName && u.Status == true);

                if (isThereAppneuronProductRecord != null)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedAppneuronProduct = new AppneuronProduct
                {
                    ProductName = request.ProductName
                };

                await _appneuronProductRepository.AddAsync(addedAppneuronProduct);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}