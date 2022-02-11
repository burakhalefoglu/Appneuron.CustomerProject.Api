using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Customers.Commands;
using Business.Handlers.Customers.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Customers.Queries.GetCustomerQuery;
using static Business.Handlers.Customers.Queries.GetCustomersQuery;
using static Business.Handlers.Customers.Commands.CreateCustomerCommand;
using static Business.Handlers.Customers.Commands.DeleteCustomerCommand;


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
                new GetCustomerQueryHandler(_customerRepository.Object, _httpContextAccessor.Object);
            _getCustomersQueryHandler = new GetCustomersQueryHandler(_customerRepository.Object, _mediator.Object);
            _createCustomerCommandHandler =
                new CreateCustomerCommandHandler(_customerRepository.Object, _httpContextAccessor.Object);
            _deleteCustomerCommandHandler =
                new DeleteCustomerCommandHandler(_customerRepository.Object, _httpContextAccessor.Object);
        }

        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerQueryHandler _getCustomerQueryHandler;
        private GetCustomersQueryHandler _getCustomersQueryHandler;
        private CreateCustomerCommandHandler _createCustomerCommandHandler;
        private DeleteCustomerCommandHandler _deleteCustomerCommandHandler;

        [Test]
        public async Task Customer_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerQuery();

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
        public async Task Customer_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomersQuery();

            _customerRepository.Setup(x
                    => x.GetListAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>
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
            var x = await _getCustomersQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Customer_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerCommand
            {
                CustomerScaleId = 1,
                IndustryId = 2
            };

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
            var command = new CreateCustomerCommand
            {
                CustomerScaleId = 1,
                IndustryId = 2
            };

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _customerRepository.Setup(x => x.Add(It.IsAny<Customer>()));

            var x = await _createCustomerCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerNotFound);
        }

        [Test]
        public async Task Customer_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerCommand();

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
            var command = new DeleteCustomerCommand();

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer) null);

            _customerRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Customer>()));

            var x = await _deleteCustomerCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerNotFound);
        }
    }
}