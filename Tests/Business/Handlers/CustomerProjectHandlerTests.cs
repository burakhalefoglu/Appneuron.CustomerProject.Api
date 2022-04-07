using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.CustomerProjects.Commands;
using Business.Handlers.CustomerProjects.Queries;
using Core.Utilities.MessageBrokers;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectQuery;
using static Business.Handlers.CustomerProjects.Queries.GetCustomerProjectsQuery;
using static Business.Handlers.CustomerProjects.Commands.CreateCustomerProjectCommand;
using static Business.Handlers.CustomerProjects.Commands.DeleteCustomerProjectCommand;


namespace Tests.Business.Handlers;

[TestFixture]
public class CustomerProjectHandlerTests
{
    [SetUp]
    public void Setup()
    {
        _customerProjectRepository = new Mock<ICustomerProjectRepository>();
        _mediator = new Mock<IMediator>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _messageBroker = new Mock<IMessageBroker>();
        _getCustomerProjectQueryHandler =
            new GetCustomerProjectQueryHandler(_customerProjectRepository.Object,
                _mediator.Object, _httpContextAccessor.Object);
        _getCustomerProjectsQueryHandler =
            new GetCustomerProjectsQueryHandler(_customerProjectRepository.Object, _mediator.Object,
                _httpContextAccessor.Object);
        _createCustomerProjectCommandHandler =
            new CreateCustomerProjectCommandHandler(_customerProjectRepository.Object,
                _mediator.Object,
                _httpContextAccessor.Object, _messageBroker.Object);
        _deleteCustomerProjectCommandHandler =
            new DeleteCustomerProjectCommandHandler(_customerProjectRepository.Object,
                _httpContextAccessor.Object, _messageBroker.Object);
    }

    private Mock<ICustomerProjectRepository> _customerProjectRepository;
    private Mock<IMediator> _mediator;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<IMessageBroker> _messageBroker;

    private GetCustomerProjectQueryHandler _getCustomerProjectQueryHandler;
    private GetCustomerProjectsQueryHandler _getCustomerProjectsQueryHandler;
    private CreateCustomerProjectCommandHandler _createCustomerProjectCommandHandler;
    private DeleteCustomerProjectCommandHandler _deleteCustomerProjectCommandHandler;

    [Test]
    public async Task CustomerProject_GetQuery_Success()
    {
        //Arrange
        var query = new GetCustomerProjectQuery
        {
            Name = "test"
        };

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());


        _customerProjectRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
            .ReturnsAsync(new CustomerProject
                {
                    Id = 2,
                    CreatedAt = DateTime.Now,
                    Name = "test"
                }
            );

        //Act
        var x = await _getCustomerProjectQueryHandler.Handle(query, new CancellationToken());

        //Asset
        x.Success.Should().BeTrue();
        x.Data.Name.Should().Be("Test");
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
            Description = "TestBody",
            Name = "Test"
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
            Description = "TestBody",
            Name = "Test"
        };

        _customerProjectRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerProject, bool>>>()))
            .ReturnsAsync(new CustomerProject());

        _customerProjectRepository.Setup(x => x.AddAsync(It.IsAny<CustomerProject>()));

        var x = await _createCustomerProjectCommandHandler.Handle(command, new CancellationToken());

        x.Success.Should().BeFalse();
        x.Message.Should().Be(Messages.NameAlreadyExist);
    }

    [Test]
    public async Task CustomerProject_DeleteCommand_Success()
    {
        //Arrange
        var command = new DeleteCustomerProjectCommand
        {
            Name = "test"
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
            Name = "test"
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