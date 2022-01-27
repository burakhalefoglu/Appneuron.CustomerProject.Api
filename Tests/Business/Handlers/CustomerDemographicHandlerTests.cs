using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.CustomerDemographics.Commands;
using Business.Handlers.CustomerDemographics.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using static Business.Handlers.CustomerDemographics.Queries.GetCustomerDemographicQuery;
using static Business.Handlers.CustomerDemographics.Queries.GetCustomerDemographicsQuery;
using static Business.Handlers.CustomerDemographics.Commands.CreateCustomerDemographicCommand;
using static Business.Handlers.CustomerDemographics.Commands.UpdateCustomerDemographicCommand;
using static Business.Handlers.CustomerDemographics.Commands.DeleteCustomerDemographicCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerDemographicHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _customerDemographicRepository = new Mock<ICustomerDemographicRepository>();
            _mediator = new Mock<IMediator>();

            _getCustomerDemographicQueryHandler =
                new GetCustomerDemographicQueryHandler(_customerDemographicRepository.Object);
            _getCustomerDemographicsQueryHandler =
                new GetCustomerDemographicsQueryHandler(_customerDemographicRepository.Object, _mediator.Object);
            _createCustomerDemographicCommandHandler =
                new CreateCustomerDemographicCommandHandler(_customerDemographicRepository.Object, _mediator.Object);
            _updateCustomerDemographicCommandHandler =
                new UpdateCustomerDemographicCommandHandler(_customerDemographicRepository.Object);
            _deleteCustomerDemographicCommandHandler =
                new DeleteCustomerDemographicCommandHandler(_customerDemographicRepository.Object, _mediator.Object);
        }

        private Mock<ICustomerDemographicRepository> _customerDemographicRepository;
        private Mock<IMediator> _mediator;

        private GetCustomerDemographicQueryHandler _getCustomerDemographicQueryHandler;
        private GetCustomerDemographicsQueryHandler _getCustomerDemographicsQueryHandler;
        private CreateCustomerDemographicCommandHandler _createCustomerDemographicCommandHandler;
        private UpdateCustomerDemographicCommandHandler _updateCustomerDemographicCommandHandler;
        private DeleteCustomerDemographicCommandHandler _deleteCustomerDemographicCommandHandler;

        [Test]
        public async Task CustomerDemographic_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerDemographicQuery
            {
                Id = "107f191e810c19729de860ea"
            };

            _customerDemographicRepository.Setup(x => x
                    .GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .ReturnsAsync(new CustomerDemographic
                {
                    Id = new ObjectId("107f191e810c19729de860ea"),
                    CustomerDesc = "Test"
                });

            //Act
            var x = await _getCustomerDemographicQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(new ObjectId("107f191e810c19729de860ea"));
            x.Data.CustomerDesc.Should().Be("Test");
        }

        [Test]
        public async Task CustomerDemographic_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerDemographicsQuery();

            _customerDemographicRepository
                .Setup(x =>
                    x.GetListAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .ReturnsAsync(new List<CustomerDemographic>
                {
                    new()
                    {
                        CustomerDesc = "Test_desc",
                        Id = new ObjectId("107f191e810c19729de860ea")
                    },

                    new()
                    {
                        CustomerDesc = "Test2",
                        Id = new ObjectId("507f191e810c19729de860ea")
                    }
                }.AsQueryable);


            //Act
            var x = await _getCustomerDemographicsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task CustomerDemographic_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerDemographicCommand
            {
                CustomerDesc = "Test_desc"
            };

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x => x.AddAsync(It.IsAny<CustomerDemographic>()));
            var x = await _createCustomerDemographicCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task CustomerDemographic_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateCustomerDemographicCommand
            {
                CustomerDesc = "Test"
            };

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult(new CustomerDemographic()));

            _customerDemographicRepository.Setup(x => x.AddAsync(It.IsAny<CustomerDemographic>()));

            var x = await _createCustomerDemographicCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerDemographic_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerDemographicCommand();
            //command.CustomerDemographicName = "test";

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .ReturnsAsync(new CustomerDemographic());

            _customerDemographicRepository.Setup(x
                => x.UpdateAsync(It.IsAny<CustomerDemographic>(),
                    It.IsAny<Expression<Func<CustomerDemographic, bool>>>()));

            var x = await _updateCustomerDemographicCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerDemographic_UpdateCommand_CustomerDemographicNotFound()
        {
            //Arrange
            var command = new UpdateCustomerDemographicCommand
            {
                Id = "107f191e810c19729de860ea",
                Customers = new List<Customer>(),
                CustomerDesc = "Test_desc"
            };

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x
                => x.Update(It.IsAny<CustomerDemographic>(), It.IsAny<Expression<Func<CustomerDemographic, bool>>>()));

            var x = await _updateCustomerDemographicCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDemographicNotFound);
        }


        [Test]
        public async Task CustomerDemographic_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerDemographicCommand();

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .ReturnsAsync(new CustomerDemographic());

            _customerDemographicRepository.Setup(x
                => x.UpdateAsync(It.IsAny<CustomerDemographic>(),
                    It.IsAny<Expression<Func<CustomerDemographic, bool>>>()));

            var x = await _deleteCustomerDemographicCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task CustomerDemographic_DeleteCommand_CustomerDemographicNotFound()
        {
            //Arrange
            var command = new DeleteCustomerDemographicCommand();

            _customerDemographicRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x
                => x.UpdateAsync(It.IsAny<CustomerDemographic>(),
                    It.IsAny<Expression<Func<CustomerDemographic, bool>>>()));

            var x = await _deleteCustomerDemographicCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDemographicNotFound);
        }
    }
}