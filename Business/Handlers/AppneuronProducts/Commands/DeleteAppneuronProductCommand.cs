﻿using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.AppneuronProducts.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteAppneuronProductCommand : IRequest<IResult>
    {
        public short Id { get; set; }

        public class DeleteAppneuronProductCommandHandler : IRequestHandler<DeleteAppneuronProductCommand, IResult>
        {
            private readonly IAppneuronProductRepository _appneuronProductRepository;
            private readonly IMediator _mediator;

            public DeleteAppneuronProductCommandHandler(IAppneuronProductRepository appneuronProductRepository, IMediator mediator)
            {
                _appneuronProductRepository = appneuronProductRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteAppneuronProductCommand request, CancellationToken cancellationToken)
            {
                var appneuronProductToDelete = await _appneuronProductRepository.GetAsync(p => p.Id == request.Id);
                if (appneuronProductToDelete == null)
                {
                    return new ErrorResult(Messages.AppneuronProductNotFound);
                }
                _appneuronProductRepository.Delete(appneuronProductToDelete);
                await _appneuronProductRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}