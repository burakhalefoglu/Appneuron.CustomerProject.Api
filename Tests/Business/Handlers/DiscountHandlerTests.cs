using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Discounts.Commands;
using Business.Handlers.Discounts.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Discounts.Queries.GetDiscountQuery;
using static Business.Handlers.Discounts.Queries.GetDiscountsQuery;
using static Business.Handlers.Discounts.Commands.CreateDiscountCommand;
using static Business.Handlers.Discounts.Commands.UpdateDiscountCommand;
using static Business.Handlers.Discounts.Commands.DeleteDiscountCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class DiscountHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _discountRepository = new Mock<IDiscountRepository>();
            _mediator = new Mock<IMediator>();

            _getDiscountQueryHandler = new GetDiscountQueryHandler(_discountRepository.Object);
            _getDiscountsQueryHandler = new GetDiscountsQueryHandler(_discountRepository.Object, _mediator.Object);
            _createDiscountCommandHandler =
                new CreateDiscountCommandHandler(_discountRepository.Object);
            _updateDiscountCommandHandler =
                new UpdateDiscountCommandHandler(_discountRepository.Object);
            _deleteDiscountCommandHandler =
                new DeleteDiscountCommandHandler(_discountRepository.Object);
        }

        private Mock<IDiscountRepository> _discountRepository;
        private Mock<IMediator> _mediator;

        private GetDiscountQueryHandler _getDiscountQueryHandler;
        private GetDiscountsQueryHandler _getDiscountsQueryHandler;
        private CreateDiscountCommandHandler _createDiscountCommandHandler;
        private UpdateDiscountCommandHandler _updateDiscountCommandHandler;
        private DeleteDiscountCommandHandler _deleteDiscountCommandHandler;

        [Test]
        public async Task Discount_GetQuery_Success()
        {
            //Arrange
            var query = new GetDiscountQuery
            {
                Id = 1
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>())).ReturnsAsync(
                new Discount
                {
                    Id =1,
                    DiscountName = "Test"
                });

            //Act
            var x = await _getDiscountQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task Discount_GetQueries_Success()
        {
            //Arrange
            var query = new GetDiscountsQuery();

            _discountRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync(new List<Discount>
                {
                    new()
                    {
                        DiscountName = "Test1",
                        Id = 1
                    },

                    new()
                    {
                        DiscountName = "Test2",
                        Id = 2
                    }
                }.AsQueryable());

            //Act
            var x = await _getDiscountsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Discount_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateDiscountCommand
            {
                Percent = 25,
                DiscountName = "Test"
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync((Discount) null);

            _discountRepository.Setup(x => x.AddAsync(It.IsAny<Discount>()));

            var x = await _createDiscountCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Discount_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateDiscountCommand
            {
                Percent = 25,
                DiscountName = "Test"
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync(new Discount());

            _discountRepository.Setup(x => x.Add(It.IsAny<Discount>()));

            var x = await _createDiscountCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Discount_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateDiscountCommand
            {
                DiscountName = "Test10",
                Id = 1,
                Percent = 40
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync(new Discount
                {
                    DiscountName = "Test",
                    Id = 1
                });

            _discountRepository.Setup(x =>
                x.Update(It.IsAny<Discount>()));

            var x = await _updateDiscountCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Discount_UpdateCommand_DiscountNotFound()
        {
            //Arrange
            var command = new UpdateDiscountCommand
            {
                DiscountName = "Test10",
                Id = 1,
                Percent = 40
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync((Discount) null);

            _discountRepository.Setup(x
                => x.Update(It.IsAny<Discount>()));

            var x = await _updateDiscountCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.DiscountNotFound);
        }


        [Test]
        public async Task Discount_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteDiscountCommand
            {
                Id = 1
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync(new Discount
                {
                    Id = 1,
                    Percent = 20
                });

            _discountRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Discount>()));

            var x = await _deleteDiscountCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Discount_DeleteCommand_DiscountNotFound()
        {
            //Arrange
            var command = new DeleteDiscountCommand
            {
                Id = 1
            };

            _discountRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>()))
                .ReturnsAsync((Discount) null);

            _discountRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Discount>()));

            var x = await _deleteDiscountCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.DiscountNotFound);
        }
    }
}