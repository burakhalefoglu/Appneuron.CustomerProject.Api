using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.CustomerProjects.Commands;
using Business.Handlers.CustomerProjects.Queries;
using Business.MessageBrokers;
using Core.Utilities.MessageBrokers;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectQuery;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectLookupQuery;
using static Business.Handlers.CustomerProjects.Queries.GetProjectCountQuery;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectsQuery;
using static Business.Handlers.CustomerProjects.Commands.CreateCustomerProjectCommand;
using static Business.Handlers.CustomerProjects.Commands.UpdateCustomerProjectCommand;
using static Business.Handlers.CustomerProjects.Commands.DeleteCustomerProjectCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerProjectHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _customerProjectRepository = new Mock<ICustomerProjectRepository>();
            _mediator = new Mock<IMediator>();
            _kafkaMessageBroker = new Mock<IMessageBroker>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerProjectQueryHandler =
                new GetCustomerProjectQueryHandler(_customerProjectRepository.Object,
                    _mediator.Object, _httpContextAccessor.Object);
            _getCustomerProjectsQueryHandler =
                new GetCustomerProjectsQueryHandler(_customerProjectRepository.Object, _mediator.Object,
                    _httpContextAccessor.Object);
            _createCustomerProjectCommandHandler =
                new CreateCustomerProjectCommandHandler(_customerProjectRepository.Object,
                    _kafkaMessageBroker.Object, _httpContextAccessor.Object);
            _updateCustomerProjectCommandHandler =
                new UpdateCustomerProjectCommandHandler(_customerProjectRepository.Object, _mediator.Object,
                    _httpContextAccessor.Object);
            _deleteCustomerProjectCommandHandler =
                new DeleteCustomerProjectCommandHandler(_customerProjectRepository.Object,
                    _httpContextAccessor.Object);
            _getProjectCountQueryHandler =
                new GetProjectCountQueryHandler(_customerProjectRepository.Object,
                    _httpContextAccessor.Object);
            _getCustomerProjectLookupQueryHandler =
                new GetCustomerProjectLookupQueryHandler(_customerProjectRepository.Object,
                    _httpContextAccessor.Object);
        }

        private Mock<ICustomerProjectRepository> _customerProjectRepository;
        private Mock<IMediator> _mediator;
        private Mock<IMessageBroker> _kafkaMessageBroker;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerProjectQueryHandler _getCustomerProjectQueryHandler;
        private GetCustomerProjectsQueryHandler _getCustomerProjectsQueryHandler;
        private CreateCustomerProjectCommandHandler _createCustomerProjectCommandHandler;
        private UpdateCustomerProjectCommandHandler _updateCustomerProjectCommandHandler;
        private DeleteCustomerProjectCommandHandler _deleteCustomerProjectCommandHandler;
        private GetProjectCountQueryHandler _getProjectCountQueryHandler;
        private GetCustomerProjectLookupQueryHandler _getCustomerProjectLookupQueryHandler;

        [Test]
        public async Task CustomerProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerProjectQuery
            {
                ProjectId = 1
            };

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());


            _customerProjectRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new CustomerProject
                    {
                        Id = 2,
                        CreatedAt = DateTime.Now,
                        ProjectName = "Test",
                    }
                );

            //Act
            var x = await _getCustomerProjectQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ProjectName.Should().Be("Test");
        }

        [Test]
        public async Task CustomerProject_GetCustomerProjectLookup_Success()
        {
            //Arrange
            var query = new GetCustomerProjectLookupQuery();

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());


            _customerProjectRepository.Setup(x => x.GetListAsync(
                    It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new List<CustomerProject>
                    {
                        new()
                        {
                            Id =  1,
                            CreatedAt = DateTime.Now,
                            ProjectName = "Test"
                        },

                        new()
                        {
                            Id = 2,
                            CreatedAt = DateTime.Now,
                            ProjectName = "Test"
                            
                        }
                    }.AsQueryable
                );

            //Act
            var x = await _getCustomerProjectLookupQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count().Should().BeGreaterThan(1);
        }


        [Test]
        public async Task CustomerProject_GetProjectCountQuery_Success()
        {
            //Arrange
            var query = new GetProjectCountQuery();

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());


            _customerProjectRepository.Setup(x => x.GetListAsync(
                    It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new List<CustomerProject>
                {
                    new()
                    {
                        Id = 1
                    },
                    new()
                    {
                        Id = 2
                    }
                }.AsQueryable);
            //Act
            var x = await _getProjectCountQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Should().Be(2);
        }

        [Test]
        public async Task CustomerProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerProjectsQuery();

            _customerProjectRepository.Setup(x
                    => x.GetListAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new List<CustomerProject>
                {
                    new()
                    {
                        Id = 1
                    },
                    new()
                    {
                        Id = 2
                    }
                }.AsQueryable);


            //Act
            var x = await _getCustomerProjectsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task CustomerProject_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerProjectCommand
            {
                ProjectBody = "TestBody",
                ProjectName = "Test"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync((CustomerProject) null);

            _customerProjectRepository.Setup(x => x.AddAsync(It.IsAny<CustomerProject>()));

            var x = await _createCustomerProjectCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task CustomerProject_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateCustomerProjectCommand
            {
                ProjectBody = "TestBody",
                ProjectName = "Test"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new CustomerProject());

            _customerProjectRepository.Setup(x => x.AddAsync(It.IsAny<CustomerProject>()));

            var x = await _createCustomerProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerProject_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerProjectCommand
            {
                ProjectId = 1
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new CustomerProject
                {
                    Id = 1
                });

            _customerProjectRepository.Setup(x => x.Update(It.IsAny<CustomerProject>()));

            var x = await _updateCustomerProjectCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerProject_UpdateCommand_ProjectNotFound()
        {
            //Arrange
            var command = new UpdateCustomerProjectCommand
            {
                ProjectId = 1
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync((CustomerProject) null);

            _customerProjectRepository.Setup(x =>
                x.Update(It.IsAny<CustomerProject>()));

            var x = await _updateCustomerProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task CustomerProject_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerProjectCommand
            {
                Id = 1
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(new CustomerProject
                {
                    Id = 1
                });

            _customerProjectRepository.Setup(x
                => x.UpdateAsync(It.IsAny<CustomerProject>()));

            var x = await _deleteCustomerProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task CustomerProject_DeleteCommand_ProjectNotFound()
        {
            //Arrange
            var command = new DeleteCustomerProjectCommand
            {
                Id = 1
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync((CustomerProject) null);

            _customerProjectRepository.Setup(x
                => x.UpdateAsync(It.IsAny<CustomerProject>()));

            var x = await _deleteCustomerProjectCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }
    }
}