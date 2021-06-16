
using Business.Handlers.Clients.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Clients.Queries.GetClientQuery;
using Entities.Concrete;
using static Business.Handlers.Clients.Queries.GetClientsQuery;
using static Business.Handlers.Clients.Commands.CreateClientCommand;
using Business.Handlers.Clients.Commands;
using Business.Constants;
using static Business.Handlers.Clients.Commands.UpdateClientCommand;
using static Business.Handlers.Clients.Commands.DeleteClientCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class ClientHandlerTests
    {
        Mock<IClientRepository> _clientRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Client_GetQuery_Success()
        {
            //Arrange
            var query = new GetClientQuery();

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>())).ReturnsAsync(new Client()
//propertyler buraya yazılacak
//{																		
//ClientId = 1,
//ClientName = "Test"
//}
);

            var handler = new GetClientQueryHandler(_clientRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.ClientId.Should().Be(1);

        }

        [Test]
        public async Task Client_GetQueries_Success()
        {
            //Arrange
            var query = new GetClientsQuery();

            _clientRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                        .ReturnsAsync(new List<Client> { new Client() { /*TODO:propertyler buraya yazılacak ClientId = 1, ClientName = "test"*/ } });

            var handler = new GetClientsQueryHandler(_clientRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Client>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Client_CreateCommand_Success()
        {
            Client rt = null;
            //Arrange
            var command = new CreateClientCommand();
            //propertyler buraya yazılacak
            //command.ClientName = "deneme";

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                        .ReturnsAsync(rt);

            _clientRepository.Setup(x => x.Add(It.IsAny<Client>())).Returns(new Client());

            var handler = new CreateClientCommandHandler(_clientRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _clientRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Client_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateClientCommand();
            //propertyler buraya yazılacak 
            //command.ClientName = "test";

            _clientRepository.Setup(x => x.Query())
                                           .Returns(new List<Client> { new Client() { /*TODO:propertyler buraya yazılacak ClientId = 1, ClientName = "test"*/ } }.AsQueryable());

            _clientRepository.Setup(x => x.Add(It.IsAny<Client>())).Returns(new Client());

            var handler = new CreateClientCommandHandler(_clientRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Client_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateClientCommand();
            //command.ClientName = "test";

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                        .ReturnsAsync(new Client() { /*TODO:propertyler buraya yazılacak ClientId = 1, ClientName = "deneme"*/ });

            _clientRepository.Setup(x => x.Update(It.IsAny<Client>())).Returns(new Client());

            var handler = new UpdateClientCommandHandler(_clientRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _clientRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Client_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteClientCommand();

            _clientRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Client, bool>>>()))
                        .ReturnsAsync(new Client() { /*TODO:propertyler buraya yazılacak ClientId = 1, ClientName = "deneme"*/});

            _clientRepository.Setup(x => x.Delete(It.IsAny<Client>()));

            var handler = new DeleteClientCommandHandler(_clientRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _clientRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

