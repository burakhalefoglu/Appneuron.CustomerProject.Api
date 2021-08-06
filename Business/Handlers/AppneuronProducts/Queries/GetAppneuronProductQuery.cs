using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.AppneuronProducts.Queries
{
    public class GetAppneuronProductQuery : IRequest<IDataResult<AppneuronProduct>>
    {
        public short Id { get; set; }

        public class GetAppneuronProductQueryHandler : IRequestHandler<GetAppneuronProductQuery, IDataResult<AppneuronProduct>>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public GetAppneuronProductQueryHandler(IAppneuronProductRepository appneuronProductRepository, IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(ApacheKafkaCustomerProjectLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AppneuronProduct>> Handle(GetAppneuronProductQuery request, CancellationToken cancellationToken)
            {
                var appneuronProduct = await _appneuronProductRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<AppneuronProduct>(appneuronProduct);
            }
        }
    }
}