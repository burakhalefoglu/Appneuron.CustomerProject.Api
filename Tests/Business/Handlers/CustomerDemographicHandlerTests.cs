
using Business.Handlers.CustomerDemographics.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.CustomerDemographics.Queries.GetCustomerDemographicQuery;
using Entities.Concrete;
using static Business.Handlers.CustomerDemographics.Queries.GetCustomerDemographicsQuery;
using static Business.Handlers.CustomerDemographics.Commands.CreateCustomerDemographicCommand;
using Business.Handlers.CustomerDemographics.Commands;
using Business.Constants;
using static Business.Handlers.CustomerDemographics.Commands.UpdateCustomerDemographicCommand;
using static Business.Handlers.CustomerDemographics.Commands.DeleteCustomerDemographicCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerDemographicHandlerTests
    {
        private Mock<ICustomerDemographicRepository> _customerDemographicRepository;
        private Mock<IMediator> _mediator;

        private GetCustomerDemographicQueryHandler _getCustomerDemographicQueryHandler;
        private GetCustomerDemographicsQueryHandler _getCustomerDemographicsQueryHandler;
        private CreateCustomerDemographicCommandHandler _createCustomerDemographicCommandHandler;
        private UpdateCustomerDemographicCommandHandler _updateCustomerDemographicCommandHandler;
        private DeleteCustomerDemographicCommandHandler _deleteCustomerDemographicCommandHandler;

        [SetUp]
        public void Setup()
        {
            _customerDemographicRepository = new Mock<ICustomerDemographicRepository>();
            _mediator = new Mock<IMediator>();

            _getCustomerDemographicQueryHandler  = new GetCustomerDemographicQueryHandler(_customerDemographicRepository.Object, _mediator.Object);
            _getCustomerDemographicsQueryHandler = new GetCustomerDemographicsQueryHandler(_customerDemographicRepository.Object, _mediator.Object);
            _createCustomerDemographicCommandHandler = new CreateCustomerDemographicCommandHandler(_customerDemographicRepository.Object, _mediator.Object);
            _updateCustomerDemographicCommandHandler = new UpdateCustomerDemographicCommandHandler(_customerDemographicRepository.Object, _mediator.Object);
            _deleteCustomerDemographicCommandHandler = new DeleteCustomerDemographicCommandHandler(_customerDemographicRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task CustomerDemographic_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerDemographicQuery
            {
                Id = 1
            };

            _customerDemographicRepository.Setup(x => x
                .GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .ReturnsAsync(new CustomerDemographic
                {
                    Id = 1,
                    CustomerDesc = "Test"
                });

            //Act
            var x = await _getCustomerDemographicQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
            x.Data.CustomerDesc.Should().Be("Test");

        }

        [Test]
        public async Task CustomerDemographic_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerDemographicsQuery();

            _customerDemographicRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                        .ReturnsAsync(new List<CustomerDemographic>
                        {
                            new ()
                            {
                                CustomerDesc = "Test",
                                Id = 1
                            },
                            
                            new ()
                            {
                                CustomerDesc = "Test2",
                                Id = 2
                            },

                        });


            //Act
            var x = await _getCustomerDemographicsQueryHandler.Handle(query, new System.Threading.CancellationToken());

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
                CustomerDesc = "Test"
            };

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                        .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x => x.Add(It.IsAny<CustomerDemographic>())).Returns(new CustomerDemographic());

            var x = await _createCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDemographicRepository.Verify(x => x.SaveChangesAsync());
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

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult(new CustomerDemographic()));

            _customerDemographicRepository.Setup(x => x.Add(It.IsAny<CustomerDemographic>())).Returns(new CustomerDemographic());

            var x = await _createCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerDemographic_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerDemographicCommand();
            //command.CustomerDemographicName = "test";

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                        .ReturnsAsync(new CustomerDemographic());

            _customerDemographicRepository.Setup(x => x.Update(It.IsAny<CustomerDemographic>())).Returns(new CustomerDemographic());

            var x = await _updateCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDemographicRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerDemographic_UpdateCommand_CustomerDemographicNotFound()
        {
            //Arrange
            var command = new UpdateCustomerDemographicCommand();
            command.Id = 1;
            command.Customers = new List<Customer>();
            command.CustomerDesc = "Test";

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                        .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x => x.Update(It.IsAny<CustomerDemographic>())).Returns(new CustomerDemographic());

            var x = await _updateCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDemographicNotFound);
        }


        [Test]
        public async Task CustomerDemographic_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerDemographicCommand();

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                        .ReturnsAsync(new CustomerDemographic()
                        {
                        });

            _customerDemographicRepository.Setup(x => x.Delete(It.IsAny<CustomerDemographic>()));

            var x = await _deleteCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDemographicRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }



        [Test]
        public async Task CustomerDemographic_DeleteCommand_CustomerDemographicNotFound()
        {
            //Arrange
            var command = new DeleteCustomerDemographicCommand();

            _customerDemographicRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDemographic, bool>>>()))
                .Returns(Task.FromResult<CustomerDemographic>(null));

            _customerDemographicRepository.Setup(x => x.Delete(It.IsAny<CustomerDemographic>()));

            var x = await _deleteCustomerDemographicCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDemographicNotFound);
        }
    }
}

