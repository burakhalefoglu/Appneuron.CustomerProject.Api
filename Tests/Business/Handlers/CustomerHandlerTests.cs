using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Internals.Handlers.Customers.Commands;
using Business.Internals.Handlers.Customers.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Internals.Handlers.Customers.Queries.GetCustomerInternalQuery;
using static Business.Internals.Handlers.Customers.Commands.CreateCustomerInternalCommand;
using static Business.Internals.Handlers.Customers.Commands.DeleteCustomerInternalCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerQueryHandler =
                new GetCustomerInternalQueryHandler(_customerRepository.Object, _httpContextAccessor.Object);
            _createCustomerCommandHandler =
                new CreateCustomerCommandInternalHandler(_customerRepository.Object, _httpContextAccessor.Object);
            _deleteCustomerCommandHandler =
                new DeleteCustomerInternalCommandHandler(_customerRepository.Object, _httpContextAccessor.Object);
        }

        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerInternalQueryHandler _getCustomerQueryHandler;
        private CreateCustomerCommandInternalHandler _createCustomerCommandHandler;
        private DeleteCustomerInternalCommandHandler _deleteCustomerCommandHandler;

        [Test]
        public async Task Customer_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerInternalQuery();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>())).ReturnsAsync(
                new Customer
                {
                    Id = 1
                });

            //Act
            var x = await _getCustomerQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }


   

        [Test]
        public async Task Customer_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerInternalCommand();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer) null);

            _customerRepository.Setup(x => x.Add(It.IsAny<Customer>()));

            var x = await _createCustomerCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Customer_CreateCommand_CustomerNotFound()
        {
            //Arrange
            var command = new CreateCustomerInternalCommand();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _customerRepository.Setup(x => x.Add(It.IsAny<Customer>()));

            var x = await _createCustomerCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.UserNotFound);
        }

        [Test]
        public async Task Customer_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerInternalCommand();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _customerRepository.Setup(x
                => x.UpdateAsync(It.IsAny<Customer>()));

            var x = await _deleteCustomerCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task Customer_DeleteCommand_CustomerNotFound()
        {
            //Arrange
            var command = new DeleteCustomerInternalCommand();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer) null);

            _customerRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Customer>()));

            var x = await _deleteCustomerCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.UserNotFound);
        }
    }
}