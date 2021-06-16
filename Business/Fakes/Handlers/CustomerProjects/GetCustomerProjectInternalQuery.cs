﻿using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Fakes.Handlers.CustomerProjects
{
    public class GetCustomerProjectInternalQuery : IRequest<IDataResult<CustomerProject>>
    {
        public string ProjectKey { get; set; }

        public class GetCustomerProjectQueryHandler : IRequestHandler<GetCustomerProjectInternalQuery, IDataResult<CustomerProject>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;

            public GetCustomerProjectQueryHandler(ICustomerProjectRepository customerProjectRepository
                 )
            {
                _customerProjectRepository = customerProjectRepository;
            }

            public async Task<IDataResult<CustomerProject>> Handle(GetCustomerProjectInternalQuery request, CancellationToken cancellationToken)
            {
                var customerProject = await _customerProjectRepository.GetAsync(p => p.ProjectKey == request.ProjectKey);
                return new SuccessDataResult<CustomerProject>(customerProject);
            }
        }
    }
}
