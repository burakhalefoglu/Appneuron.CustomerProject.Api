
using Business.Handlers.CustomerProjects.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Entities.Concrete;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectQuery;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectLookupQuery;
using static Business.Handlers.CustomerProjects.Queries.GetProjectCountQuery;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectsQuery;
using static Business.Handlers.CustomerProjects.Commands.CreateCustomerProjectCommand;
using Business.Handlers.CustomerProjects.Commands;
using Business.Constants;
using static Business.Handlers.CustomerProjects.Commands.UpdateCustomerProjectCommand;
using static Business.Handlers.CustomerProjects.Commands.DeleteCustomerProjectCommand;
using MediatR;
using System.Linq;
using Business.MessageBrokers.Kafka;
using FluentAssertions;
using Microsoft.AspNetCore.Http;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerProjectHandlerTests
    {
        private Mock<ICustomerProjectRepository> _customerProjectRepository;
        private Mock<IMediator> _mediator;
        private Mock<IKafkaMessageBroker> _kafkaMessageBroker;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerProjectQueryHandler _getCustomerProjectQueryHandler;
        private GetCustomerProjectsQueryHandler _getCustomerProjectsQueryHandler;
        private CreateCustomerProjectCommandHandler _createCustomerProjectCommandHandler;
        private UpdateCustomerProjectCommandHandler _updateCustomerProjectCommandHandler;
        private DeleteCustomerProjectCommandHandler _deleteCustomerProjectCommandHandler;
        private GetProjectCountQueryHandler _getProjectCountQueryHandler;
        private GetCustomerProjectLookupQueryHandler _getCustomerProjectLookupQueryHandler;

        [SetUp]
        public void Setup()
        {
            _customerProjectRepository = new Mock<ICustomerProjectRepository>();
            _mediator = new Mock<IMediator>();
            _kafkaMessageBroker = new Mock<IKafkaMessageBroker>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerProjectQueryHandler =
                new GetCustomerProjectQueryHandler(_customerProjectRepository.Object,
                    _mediator.Object, _httpContextAccessor.Object);
            _getCustomerProjectsQueryHandler =
                new GetCustomerProjectsQueryHandler(_customerProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);
            _createCustomerProjectCommandHandler =
                new CreateCustomerProjectCommandHandler(_customerProjectRepository.Object, _mediator.Object,
                    _kafkaMessageBroker.Object, _httpContextAccessor.Object);
            _updateCustomerProjectCommandHandler =
                new UpdateCustomerProjectCommandHandler(_customerProjectRepository.Object, _mediator.Object,
                    _httpContextAccessor.Object);
            _deleteCustomerProjectCommandHandler =
                new DeleteCustomerProjectCommandHandler(_customerProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);
            _getProjectCountQueryHandler =
                new GetProjectCountQueryHandler(_customerProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);
            _getCustomerProjectLookupQueryHandler =
                new GetCustomerProjectLookupQueryHandler(_customerProjectRepository.Object, _mediator.Object, _httpContextAccessor.Object);

        }

        [Test]
        public async Task CustomerProject_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerProjectQuery();
            query.ProjectKey = "TestKey";
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());


            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>())).ReturnsAsync(new CustomerProject()
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                ProjectName = "Test",
                ProjectKey = "TestKey"
            }
            );

            //Act
            var x = await _getCustomerProjectQueryHandler.Handle(query, new System.Threading.CancellationToken());

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
                
                .ReturnsAsync(new List<CustomerProject>()
                {
                    new()
                    {
                        Id = 1,
                        CreatedAt = DateTime.Now,
                        ProjectName = "Test",
                        ProjectKey = "TestKey"
                    },

                    new()
                    {
                        Id = 1,
                        CreatedAt = DateTime.Now,
                        ProjectName = "Test",
                        ProjectKey = "TestKey"
                    },


                }
            );

            //Act
            var x = await _getCustomerProjectLookupQueryHandler.Handle(query, new System.Threading.CancellationToken());

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


            _customerProjectRepository.Setup(x => x.GetCountAsync(
                    It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync(5);

            //Act
            var x = await _getProjectCountQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Should().Be(5);

        }

        [Test]
        public async Task CustomerProject_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerProjectsQuery();

            _customerProjectRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                        .ReturnsAsync(new List<CustomerProject>
                        {
                            new()
                            {
                                ProjectKey = "TestKey1"
                            },
                            new()
                            {
                                ProjectKey = "TestKey2"
                            }
                        });


            //Act
            var x = await _getCustomerProjectsQueryHandler.Handle(query, new System.Threading.CancellationToken());

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
                        .ReturnsAsync((CustomerProject)null);

            _customerProjectRepository.Setup(x => x.Add(It.IsAny<CustomerProject>())).Returns(new CustomerProject());

            var x = await _createCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerProjectRepository.Verify(x => x.SaveChangesAsync());
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

            _customerProjectRepository.Setup(x => x.Add(It.IsAny<CustomerProject>())).Returns(new CustomerProject());

            var x = await _createCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerProject_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerProjectCommand
            {
                ProjectKey = "test"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                        .ReturnsAsync(new CustomerProject()
                        {
                            ProjectKey = "test"
                        });

            _customerProjectRepository.Setup(x => x.Update(It.IsAny<CustomerProject>())).Returns(new CustomerProject());

            var x = await _updateCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerProject_UpdateCommand_ProjectNotFound()
        {
            //Arrange
            var command = new UpdateCustomerProjectCommand
            {
                ProjectKey = "test"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                        .ReturnsAsync((CustomerProject)null);

            _customerProjectRepository.Setup(x => x.Update(It.IsAny<CustomerProject>())).Returns(new CustomerProject());

            var x = await _updateCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task CustomerProject_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerProjectCommand
            {
                Id = "idTest"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                        .ReturnsAsync(new CustomerProject()
                        {
                            ProjectKey = "idTest"
                        });

            _customerProjectRepository.Setup(x => x.Delete(It.IsAny<CustomerProject>()));

            var x = await _deleteCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerProjectRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task CustomerProject_DeleteCommand_ProjectNotFound()
        {
            //Arrange
            var command = new DeleteCustomerProjectCommand
            {
                Id = "idTest"
            };

            _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
                .ReturnsAsync((CustomerProject)null);

            _customerProjectRepository.Setup(x => x.Delete(It.IsAny<CustomerProject>()));

            var x = await _deleteCustomerProjectCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }
    }
}

