﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Customers.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerCommand : IRequest<IResult>
    {
        public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public DeleteCustomerCommandHandler(ICustomerRepository customerRepository,
                IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var customerToDelete = await _customerRepository.GetAsync(p => p.UserId == userId);
                if (customerToDelete == null) return new ErrorResult(Messages.CustomerNotFound);

                _customerRepository.Delete(customerToDelete);
                await _customerRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}