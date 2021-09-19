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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.AppneuronProducts.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateAppneuronProductCommand : IRequest<IResult>
    {
        public string ProductName { get; set; }

        public class CreateAppneuronProductCommandHandler : IRequestHandler<CreateAppneuronProductCommand, IResult>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public CreateAppneuronProductCommandHandler(IAppneuronProductRepository appneuronProductRepository, IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateAppneuronProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateAppneuronProductCommand request, CancellationToken cancellationToken)
            {
                var isThereAppneuronProductRecord = _appneuronProductRepository.Query().Any(u => u.ProductName == request.ProductName);

                if (isThereAppneuronProductRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedAppneuronProduct = new AppneuronProduct
                {
                    ProductName = request.ProductName,
                };

                _appneuronProductRepository.Add(addedAppneuronProduct);
                await _appneuronProductRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}