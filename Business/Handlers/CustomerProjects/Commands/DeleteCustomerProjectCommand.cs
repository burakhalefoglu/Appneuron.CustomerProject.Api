﻿using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class DeleteCustomerProjectCommand : IRequest<IResult>
    {
        public long Id { get; set; }

        public class DeleteCustomerProjectCommandHandler : IRequestHandler<DeleteCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public DeleteCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                int userId = int.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var customerProjectToDelete = _customerProjectRepository.Get(p => p.Id == request.Id && p.CustomerId == userId);

                _customerProjectRepository.Delete(customerProjectToDelete);
                await _customerProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}