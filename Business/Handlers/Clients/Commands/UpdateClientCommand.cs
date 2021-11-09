﻿using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Clients.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Clients.Commands
{
    public class UpdateClientCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public long ProjectId { get; set; }
        public System.DateTime CreatedAt { get; set; }
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
                var isThereClientRecord = await _clientRepository.GetAsync(u => u.Id == request.Id);

                if (isThereClientRecord == null)
                {
                    return new ErrorResult(Messages.ClientNotFound);
                }

                isThereClientRecord.ClientId = request.ClientId;
                isThereClientRecord.ProjectId = request.ProjectId;
                isThereClientRecord.CreatedAt = request.CreatedAt;
                isThereClientRecord.IsPaidClient = request.IsPaidClient;

                _clientRepository.Update(isThereClientRecord);
                await _clientRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}