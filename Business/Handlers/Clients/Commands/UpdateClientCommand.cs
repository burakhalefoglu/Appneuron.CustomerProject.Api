using System;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Clients.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Clients.Commands
{
    public class UpdateClientCommand : IRequest<IResult>
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }

        public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, IResult>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IMediator _mediator;

            public UpdateClientCommandHandler(IClientRepository clientRepository, IMediator mediator)
            {
                _clientRepository = clientRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateClientValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
            {
                var isThereClientRecord = await _clientRepository.GetAsync(u => u.ObjectId == request.Id);

                if (isThereClientRecord == null) return new ErrorResult(Messages.ClientNotFound);

                isThereClientRecord.ClientId = request.ClientId;
                isThereClientRecord.ProjectId = request.ProjectId;
                isThereClientRecord.CreatedAt = request.CreatedAt;
                isThereClientRecord.IsPaidClient = request.IsPaidClient;

                await _clientRepository.UpdateAsync(isThereClientRecord,
                    x => x.ObjectId == isThereClientRecord.ObjectId);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}