
using Business.Handlers.CustomerDiscounts.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.CustomerDiscounts.Queries.GetCustomerDiscountQuery;
using Entities.Concrete;
using static Business.Handlers.CustomerDiscounts.Queries.GetCustomerDiscountsQuery;
using static Business.Handlers.CustomerDiscounts.Commands.CreateCustomerDiscountCommand;
using Business.Handlers.CustomerDiscounts.Commands;
using Business.Constants;
using static Business.Handlers.CustomerDiscounts.Commands.UpdateCustomerDiscountCommand;
using static Business.Handlers.CustomerDiscounts.Commands.DeleteCustomerDiscountCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerDiscountHandlerTests
    {
        private Mock<ICustomerDiscountRepository> _customerDiscountRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerDiscountQueryHandler _getCustomerDiscountQueryHandler;
        private GetCustomerDiscountsQueryHandler _getCustomerDiscountsQueryHandler;
        private CreateCustomerDiscountCommandHandler _createCustomerDiscountCommandHandler;
        private UpdateCustomerDiscountCommandHandler _updateCustomerDiscountCommandHandler;
        private DeleteCustomerDiscountCommandHandler _deleteCustomerDiscountCommandHandler;


        [SetUp]
        public void Setup()
        {
            _customerDiscountRepository = new Mock<ICustomerDiscountRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerDiscountQueryHandler = new GetCustomerDiscountQueryHandler(_customerDiscountRepository.Object, _mediator.Object, _httpContextAccessor.Object);
            _getCustomerDiscountsQueryHandler = new GetCustomerDiscountsQueryHandler(_customerDiscountRepository.Object, _mediator.Object, _httpContextAccessor.Object);
            _createCustomerDiscountCommandHandler = new CreateCustomerDiscountCommandHandler(_customerDiscountRepository.Object, _mediator.Object);
            _updateCustomerDiscountCommandHandler = new UpdateCustomerDiscountCommandHandler(_customerDiscountRepository.Object, _mediator.Object);
            _deleteCustomerDiscountCommandHandler = new DeleteCustomerDiscountCommandHandler(_customerDiscountRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task CustomerDiscount_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerDiscountQuery
            {
                DiscountId = 12
            };

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>())).ReturnsAsync(new CustomerDiscount()
            {
                DiscountId = 5,
                UserId = 12
            });

            //Act
            var x = await _getCustomerDiscountQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.DiscountId.Should().Be(5);

        }

        [Test]
        public async Task CustomerDiscount_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerDiscountsQuery();

            _customerDiscountRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                        .ReturnsAsync(new List<CustomerDiscount> {
                        new ()
                        {
                            Discount = new Discount(),
                            Customer = new Customer(),
                            Id = 1,
                            DiscountId = 12,
                            UserId = 12
                        },

                        new ()
                        {
                            Discount = new Discount(),
                            Customer = new Customer(),
                            Id = 2,
                            DiscountId = 9,
                            UserId = 10
                        },

                        });

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            //Act
            var x = await _getCustomerDiscountsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task CustomerDiscount_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerDiscountCommand
            {
                DiscountId = 12,
                CustomerId = 10
            };


            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                        .ReturnsAsync((CustomerDiscount)null);

            _customerDiscountRepository.Setup(x => x.Add(It.IsAny<CustomerDiscount>())).Returns(new CustomerDiscount());

            var x = await _createCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDiscountRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task CustomerDiscount_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateCustomerDiscountCommand
            {
                DiscountId = 12,
                CustomerId = 10
            };


            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync(new CustomerDiscount());

            _customerDiscountRepository.Setup(x => x.Add(It.IsAny<CustomerDiscount>())).Returns(new CustomerDiscount());

            var x = await _createCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerDiscount_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerDiscountCommand
            {
                Id = 1,
                DiscountId = 12,
                CustomerId = 10
            };

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                        .ReturnsAsync(new CustomerDiscount()
                        {
                            UserId = 1,
                            Id = 2
                        });

            _customerDiscountRepository.Setup(x => x.Update(It.IsAny<CustomerDiscount>())).Returns(new CustomerDiscount());

            var x = await _updateCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDiscountRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerDiscount_UpdateCommand_CustomerDiscountNotFound()
        {
            //Arrange
            var command = new UpdateCustomerDiscountCommand
            {
                Id = 1,
                DiscountId = 12,
                CustomerId = 10
            };

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync((CustomerDiscount)null);

            _customerDiscountRepository.Setup(x => x.Update(It.IsAny<CustomerDiscount>())).Returns(new CustomerDiscount());

            var x = await _updateCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDiscountNotFound);
        }

        [Test]
        public async Task CustomerDiscount_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerDiscountCommand
            {
                Id = 1
            };

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                        .ReturnsAsync(new CustomerDiscount()
                        {
                            Customer = new Customer(),
                            Discount = new Discount(),
                            Id = 1,
                            UserId = 12,
                            DiscountId = 15
                        });

            _customerDiscountRepository.Setup(x => x.Delete(It.IsAny<CustomerDiscount>()));

            var x = await _deleteCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerDiscountRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task CustomerDiscount_DeleteCommand_CustomerDiscountNotFounds()
        {
            //Arrange
            var command = new DeleteCustomerDiscountCommand
            {
                Id = 1
            };

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync((CustomerDiscount)null);

            _customerDiscountRepository.Setup(x => x.Delete(It.IsAny<CustomerDiscount>()));

            var x = await _deleteCustomerDiscountCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDiscountNotFound);
        }
    }
}

