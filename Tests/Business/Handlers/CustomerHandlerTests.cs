
using Business.Handlers.Customers.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Customers.Queries.GetCustomerQuery;
using Entities.Concrete;
using static Business.Handlers.Customers.Queries.GetCustomersQuery;
using static Business.Handlers.Customers.Commands.CreateCustomerCommand;
using Business.Handlers.Customers.Commands;
using Business.Constants;
using static Business.Handlers.Customers.Commands.DeleteCustomerCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerHandlerTests
    {
        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerQueryHandler _getCustomerQueryHandler;
        private GetCustomersQueryHandler _getCustomersQueryHandler;
        private CreateCustomerCommandHandler _createCustomerCommandHandler;
        private DeleteCustomerCommandHandler _deleteCustomerCommandHandler;

        [SetUp]
        public void Setup()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerQueryHandler = new GetCustomerQueryHandler(_customerRepository.Object, 
                _mediator.Object, _httpContextAccessor.Object);
            _getCustomersQueryHandler = new GetCustomersQueryHandler(_customerRepository.Object, _mediator.Object);
            _createCustomerCommandHandler = new CreateCustomerCommandHandler(_customerRepository.Object, _mediator.Object,_httpContextAccessor.Object );
            _deleteCustomerCommandHandler = new DeleteCustomerCommandHandler(_customerRepository.Object, 
                _mediator.Object, _httpContextAccessor.Object);

        }

        [Test]
        public async Task Customer_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerQuery();
            
            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>())).ReturnsAsync(new Customer()
            {
                UserId = 1,
            });

            //Act
            var x = await _getCustomerQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be(1);

        }


        [Test]
        public async Task Customer_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomersQuery();

            _customerRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                        .ReturnsAsync(new List<Customer> { 
                            new ()
                            {
                                UserId = 1
                            },
                            
                            new ()
                            {
                                UserId = 2
                            },

                        });

            //Act
            var x = await _getCustomersQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Customer_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerCommand();
            command.CustomerScaleId = 2;
            command.IndustryId = 1;

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                        .ReturnsAsync((Customer)null);

            _customerRepository.Setup(x => x.Add(It.IsAny<Customer>())).Returns(new Customer());

            var x = await _createCustomerCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Customer_CreateCommand_CustomerNotFound()
        {
            //Arrange
            var command = new CreateCustomerCommand();
            command.CustomerScaleId = 2;
            command.IndustryId = 1;

            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new Customer());

            _customerRepository.Setup(x => x.Add(It.IsAny<Customer>())).Returns(new Customer());

            var x = await _createCustomerCommandHandler.Handle(command, new System.Threading.CancellationToken());

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

            _customerRepository.Setup(x => x.Delete(It.IsAny<Customer>()));

            var x = await _deleteCustomerCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task Customer_DeleteCommand_CustomerNotFound()
        {
            //Arrange
            var command = new DeleteCustomerCommand();
            
            _customerRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                        .ReturnsAsync((Customer)null);

            _customerRepository.Setup(x => x.Delete(It.IsAny<Customer>()));

            var x = await _deleteCustomerCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerNotFound);
        }


    }
}

