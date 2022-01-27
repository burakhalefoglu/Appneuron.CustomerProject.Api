using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.CustomerProjects;
using Business.Handlers.Clients.Commands;
using Business.Handlers.Clients.Queries;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Clients.Commands.CreateClientCommand;
using static Business.Handlers.Clients.Commands.DeleteClientCommand;
using static Business.Handlers.Clients.Commands.UpdateClientCommand;
using static Business.Handlers.Clients.Queries.GetClientQuery;
using static Business.Handlers.Clients.Queries.GetClientsQuery;

namespace Tests.Business.Handlers
{
    [TestFixture]
    public class ClientHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();
            _mediator = new Mock<IMediator>();

            _getClientQueryHandler = new GetClientQueryHandler(_clientRepository.Object);
            _getClientsQueryHandler = new GetClientsQueryHandler(_clientRepository.Object, _mediator.Object);
            _createClientCommandHandler = new CreateClientCommandHandler(_clientRepository.Object, _mediator.Object);
            _updateClientCommandHandler = new UpdateClientCommandHandler(_clientRepository.Object, _mediator.Object);
            _deleteClientCommandHandler = new DeleteClientCommandHandler(_clientRepository.Object);
        }

        private Mock<IClientRepository> _clientRepository;
        private Mock<IMediator> _mediator;

        private GetClientQueryHandler _getClientQueryHandler;
        private GetClientsQueryHandler _getClientsQueryHandler;
        private CreateClientCommandHandler _createClientCommandHandler;
        private UpdateClientCommandHandler _updateClientCommandHandler;
        private DeleteClientCommandHandler _deleteClientCommandHandler;

        [Test]
        public async Task Client_GetQuery_Success()
        {
            //Arrange
            var query = new GetClientQuery
            {
                Id = "507f191e810c19729de860ea"
            };

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>())).ReturnsAsync(new Client
            {
                ClientId = "507f191e810c19729de860ea",
                ProjectId = "107f191e810c19729de860ea"
            });


            //Act
            var x = await _getClientQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ClientId.Should().Be("507f191e810c19729de860ea");
        }

        [Test]
        public async Task Client_GetQueries_Success()
        {
            //Arrange
            var query = new GetClientsQuery();

            _clientRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .ReturnsAsync(new List<Client>
                {
                    new()
                    {
                        ProjectId = "507f191e810c19729de860ea",
                        ClientId = "300f191e810c19729de860ea"
                    },

                    new()
                    {
                        ProjectId = "107f191e810c19729de860ea",
                        ClientId = "207f191e810c19729de860ea"
                    }
                }.AsQueryable);


            //Act
            var x = await _getClientsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Client_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateClientCommand
            {
                ClientId = "507f191e810c19729de860ea",
                ProjectId = "107f191e810c19729de860ea",
                IsPaidClient = false,
                CreatedAt = DateTime.Now
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetCustomerProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<CustomerProject>(new CustomerProject()));

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x => x.AddAsync(It.IsAny<Client>()));

            var x = await _createClientCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Client_CreateCommand_ProjectNotFound()
        {
            //Arrange
            var command = new CreateClientCommand
            {
                ClientId = "507f191e810c19729de860ea",
                ProjectId = "107f191e810c19729de860ea",
                IsPaidClient = false,
                CreatedAt = DateTime.Now
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetCustomerProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<CustomerProject>());

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x => x.AddAsync(It.IsAny<Client>()));

            var x = await _createClientCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task Client_CreateCommand_ClientAlreadyExist()
        {
            //Arrange
            var command = new CreateClientCommand
            {
                ClientId = "507f191e810c19729de860ea",
                ProjectId = "107f191e810c19729de860ea",
                IsPaidClient = false,
                CreatedAt = DateTime.Now
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetCustomerProjectInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<CustomerProject>(new CustomerProject()));

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult(new Client()));

            _clientRepository.Setup(x => x.AddAsync(It.IsAny<Client>()));

            var x = await _createClientCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ClientAlreadyExist);
        }

        [Test]
        public async Task Client_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateClientCommand
            {
                ClientId = "507f191e810c19729de860ea",
                Id = "107f191e810c19729de860ea"
            };

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .ReturnsAsync(new Client());

            _clientRepository.Setup(x => x.Update(It.IsAny<Client>(), It.IsAny<Expression<Func<Client, bool>>>()));
            var x = await _updateClientCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Client_UpdateCommand_ClientNotFound()
        {
            //Arrange
            var command = new UpdateClientCommand
            {
                ClientId = "507f191e810c19729de860ea",
                Id = "107f191e810c19729de860ea"
            };

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x => x.Update(It.IsAny<Client>(), It.IsAny<Expression<Func<Client, bool>>>()));
            var x = await _updateClientCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ClientNotFound);
        }

        [Test]
        public async Task Client_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteClientCommand
            {
                Id = "107f191e810c19729de860ea"
            };

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .ReturnsAsync(new Client());

            _clientRepository.Setup(x => x.UpdateAsync(It.IsAny<Client>(),
                It.IsAny<Expression<Func<Client, bool>>>()));
            var x = await _deleteClientCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Client_DeleteCommand_ClientNotFound()
        {
            //Arrange
            var command = new DeleteClientCommand
            {
                Id = "107f191e810c19729de860ea"
            };

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                .Returns(Task.FromResult<Client>(null));

            _clientRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Client>(), It.IsAny<Expression<Func<Client, bool>>>()));

            var x = await _deleteClientCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ClientNotFound);
        }
    }
}