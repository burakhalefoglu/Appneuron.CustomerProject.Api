﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.CustomerScales.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.CustomerScales.Commands
{
    public class UpdateCustomerScaleCommand : IRequest<IResult>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public class UpdateCustomerScaleCommandHandler : IRequestHandler<UpdateCustomerScaleCommand, IResult>
        {
            private readonly ICustomerScaleRepository _customerScaleRepository;

            public UpdateCustomerScaleCommandHandler(ICustomerScaleRepository customerScaleRepository)
            {
                _customerScaleRepository = customerScaleRepository;
            }

            [ValidationAspect(typeof(UpdateCustomerScaleValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateCustomerScaleCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerScaleRecord = await _customerScaleRepository.GetAsync(u => u.ObjectId == request.Id);
                if (isThereCustomerScaleRecord == null) return new ErrorResult(Messages.CustomerScaleNotFound);
                isThereCustomerScaleRecord.Name = request.Name;
                isThereCustomerScaleRecord.Description = request.Description;

                await _customerScaleRepository.UpdateAsync(isThereCustomerScaleRecord,
                    x => x.ObjectId == isThereCustomerScaleRecord.ObjectId);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}