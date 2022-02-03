using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.AppneuronProducts.Queries
{
    public class GetAppneuronProductQuery : IRequest<IDataResult<AppneuronProduct>>
    {
        public string Id { get; set; }

        public class
            GetAppneuronProductQueryHandler : IRequestHandler<GetAppneuronProductQuery, IDataResult<AppneuronProduct>>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public GetAppneuronProductQueryHandler(IAppneuronProductRepository appneuronProductRepository,
                IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AppneuronProduct>> Handle(GetAppneuronProductQuery request,
                CancellationToken cancellationToken)
            {
                var appneuronProduct = await _appneuronProductRepository.GetAsync(p => p.ObjectId == request.Id);
                return new SuccessDataResult<AppneuronProduct>(appneuronProduct);
            }
        }
    }
}