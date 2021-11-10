﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.CustomerDiscounts.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerDiscountCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCustomerDiscountCommandHandler : IRequestHandler<DeleteCustomerDiscountCommand, IResult>
        {
            private readonly ICustomerDiscountRepository _customerDiscountRepository;
            private readonly IMediator _mediator;

            public DeleteCustomerDiscountCommandHandler(ICustomerDiscountRepository customerDiscountRepository,
                IMediator mediator)
            {
                _customerDiscountRepository = customerDiscountRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerDiscountCommand request,
                CancellationToken cancellationToken)
            {
                var customerDiscountToDelete = await _customerDiscountRepository.GetAsync(p => p.Id == request.Id);
                if (customerDiscountToDelete == null) return new ErrorResult(Messages.CustomerDiscountNotFound);

                _customerDiscountRepository.Delete(customerDiscountToDelete);
                await _customerDiscountRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}