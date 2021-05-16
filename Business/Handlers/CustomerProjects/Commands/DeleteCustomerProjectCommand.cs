using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

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

            public DeleteCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository, IMediator mediator)
            {
                _customerProjectRepository = customerProjectRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                var customerProjectToDelete = _customerProjectRepository.Get(p => p.Id == request.Id);

                _customerProjectRepository.Delete(customerProjectToDelete);
                await _customerProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}