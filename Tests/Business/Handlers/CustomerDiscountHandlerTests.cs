using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.CustomerDiscounts.Commands;
using Business.Handlers.CustomerDiscounts.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using static Business.Handlers.CustomerDiscounts.Queries.GetCustomerDiscountQuery;
using static Business.Handlers.CustomerDiscounts.Queries.GetCustomerDiscountsQuery;
using static Business.Handlers.CustomerDiscounts.Commands.CreateCustomerDiscountCommand;
using static Business.Handlers.CustomerDiscounts.Commands.UpdateCustomerDiscountCommand;
using static Business.Handlers.CustomerDiscounts.Commands.DeleteCustomerDiscountCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerDiscountHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _customerDiscountRepository = new Mock<ICustomerDiscountRepository>();
            _mediator = new Mock<IMediator>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _getCustomerDiscountQueryHandler = new GetCustomerDiscountQueryHandler(_customerDiscountRepository.Object,
                _httpContextAccessor.Object);
            _getCustomerDiscountsQueryHandler = new GetCustomerDiscountsQueryHandler(_customerDiscountRepository.Object,
                _mediator.Object, _httpContextAccessor.Object);
            _createCustomerDiscountCommandHandler =
                new CreateCustomerDiscountCommandHandler(_customerDiscountRepository.Object);
            _updateCustomerDiscountCommandHandler =
                new UpdateCustomerDiscountCommandHandler(_customerDiscountRepository.Object);
            _deleteCustomerDiscountCommandHandler =
                new DeleteCustomerDiscountCommandHandler(_customerDiscountRepository.Object);
        }

        private Mock<ICustomerDiscountRepository> _customerDiscountRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHttpContextAccessor> _httpContextAccessor;

        private GetCustomerDiscountQueryHandler _getCustomerDiscountQueryHandler;
        private GetCustomerDiscountsQueryHandler _getCustomerDiscountsQueryHandler;
        private CreateCustomerDiscountCommandHandler _createCustomerDiscountCommandHandler;
        private UpdateCustomerDiscountCommandHandler _updateCustomerDiscountCommandHandler;
        private DeleteCustomerDiscountCommandHandler _deleteCustomerDiscountCommandHandler;

        [Test]
        public async Task CustomerDiscount_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerDiscountQuery
            {
                DiscountId = 1
            };

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync(new CustomerDiscount
                {
                    DiscountId = 1,
                    UserId = 2
                });

            //Act
            var x = await _getCustomerDiscountQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.DiscountId.Should().Be(1);
        }

        [Test]
        public async Task CustomerDiscount_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerDiscountsQuery();

            _customerDiscountRepository.Setup(x
                    => x.GetListAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync(new List<CustomerDiscount>
                {
                    new()
                    {
                        Id = 1,
                        DiscountId = 1,
                        UserId = 13
                    },

                    new()
                    {
                        Id = 2,
                        DiscountId = 3,
                        UserId = 14
                    }
                }.AsQueryable);

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            //Act
            var x = await _getCustomerDiscountsQueryHandler.Handle(query, new CancellationToken());

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
                DiscountId = 1,
                CustomerId = 2
            };


            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync((CustomerDiscount) null);

            _customerDiscountRepository.Setup(x => x.AddAsync(It.IsAny<CustomerDiscount>()));

            var x = await _createCustomerDiscountCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task CustomerDiscount_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateCustomerDiscountCommand
            {                
                DiscountId = 1,
                CustomerId = 2
            };


            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync(new CustomerDiscount());

            _customerDiscountRepository.Setup(x => x.AddAsync(It.IsAny<CustomerDiscount>()));

            var x = await _createCustomerDiscountCommandHandler.Handle(command, new CancellationToken());

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
                DiscountId = 3,
                CustomerId = 12
            };

            _customerDiscountRepository.Setup(x
                    => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync(new CustomerDiscount
                {
                    UserId = 12,
                    Id = 1
                });

            _customerDiscountRepository.Setup(x => x.UpdateAsync(It.IsAny<CustomerDiscount>()));

            var x = await _updateCustomerDiscountCommandHandler.Handle(command, new CancellationToken());
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
                DiscountId = 2,
                CustomerId = 3
            };

            _customerDiscountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerDiscount, bool>>>()))
                .ReturnsAsync((CustomerDiscount) null);

            _customerDiscountRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<CustomerDiscount>()));

            var x = await _updateCustomerDiscountCommandHandler.Handle(command, new CancellationToken());

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
                .ReturnsAsync(new CustomerDiscount
                {
                    Id = 1,
                    DiscountId = 3,
                    UserId = 12
                });

            _customerDiscountRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<CustomerDiscount>()));

            var x = await _deleteCustomerDiscountCommandHandler.Handle(command, new CancellationToken());
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
                .ReturnsAsync((CustomerDiscount) null);

            _customerDiscountRepository.Setup(x => x.UpdateAsync(It.IsAny<CustomerDiscount>()));

            var x = await _deleteCustomerDiscountCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerDiscountNotFound);
        }
    }
}