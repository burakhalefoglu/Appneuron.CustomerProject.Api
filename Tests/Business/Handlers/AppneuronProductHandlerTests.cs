using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.AppneuronProducts.Commands;
using Business.Handlers.AppneuronProducts.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using static Business.Handlers.AppneuronProducts.Queries.GetAppneuronProductQuery;
using static Business.Handlers.AppneuronProducts.Queries.GetAppneuronProductsQuery;
using static Business.Handlers.AppneuronProducts.Commands.CreateAppneuronProductCommand;
using static Business.Handlers.AppneuronProducts.Commands.UpdateAppneuronProductCommand;
using static Business.Handlers.AppneuronProducts.Commands.DeleteAppneuronProductCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class AppneuronProductHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _appneuronProductRepository = new Mock<IAppneuronProductRepository>();
            _mediator = new Mock<IMediator>();

            _getAppneuronProductQueryHandler =
                new GetAppneuronProductQueryHandler(_appneuronProductRepository.Object, _mediator.Object);
            _getAppneuronProductsQueryHandler =
                new GetAppneuronProductsQueryHandler(_appneuronProductRepository.Object, _mediator.Object);
            _createAppneuronProductCommandHandler =
                new CreateAppneuronProductCommandHandler(_appneuronProductRepository.Object);
            _updateAppneuronProductCommandHandler =
                new UpdateAppneuronProductCommandHandler(_appneuronProductRepository.Object, _mediator.Object);
            _deleteAppneuronProductCommandHandler =
                new DeleteAppneuronProductCommandHandler(_appneuronProductRepository.Object, _mediator.Object);
        }

        private Mock<IAppneuronProductRepository> _appneuronProductRepository;
        private Mock<IMediator> _mediator;

        private GetAppneuronProductQueryHandler _getAppneuronProductQueryHandler;
        private GetAppneuronProductsQueryHandler _getAppneuronProductsQueryHandler;
        private CreateAppneuronProductCommandHandler _createAppneuronProductCommandHandler;
        private UpdateAppneuronProductCommandHandler _updateAppneuronProductCommandHandler;
        private DeleteAppneuronProductCommandHandler _deleteAppneuronProductCommandHandler;

        [Test]
        public async Task AppneuronProduct_GetQuery_Success()
        {
            //Arrange
            var query = new GetAppneuronProductQuery
            {
                Id = "507f191e810c19729de860ea"
            };

            _appneuronProductRepository.Setup(x =>
                    x.GetAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .ReturnsAsync(new AppneuronProduct
                    {
                        ProductName = "Test",
                        Id = new ObjectId("507f191e810c19729de860ea")
                    }
                );


            //Act
            var x = await _getAppneuronProductQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ProductName.Should().Be("Test");
        }

        [Test]
        public async Task AppneuronProduct_GetQueries_Success()
        {
            //Arrange
            var query = new GetAppneuronProductsQuery();

            _appneuronProductRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .ReturnsAsync(new List<AppneuronProduct>
                {
                    new()
                    {
                        ProductName = "Test",
                        Id = new ObjectId("507f191e810c19729de860ea")
                    },

                    new()
                    {
                        ProductName = "Test2",
                        Id = new ObjectId("107f191e810c19729de860ea")
                    }
                }.AsQueryable());
            //Act
            var x = await _getAppneuronProductsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
             x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task AppneuronProduct_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateAppneuronProductCommand
            {
                ProductName = "Test_name"
            };

            _appneuronProductRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .Returns(Task.FromResult<AppneuronProduct>(null));

            _appneuronProductRepository.Setup(x => x.AddAsync(It.IsAny<AppneuronProduct>()));

            var x = await _createAppneuronProductCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task AppneuronProduct_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateAppneuronProductCommand
            {
                ProductName = "Test"
            };

            _appneuronProductRepository.Setup(x => x.GetAsync(
                    It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .Returns(Task.FromResult(new AppneuronProduct()));

            _appneuronProductRepository.Setup(x => x.AddAsync(It.IsAny<AppneuronProduct>()));
            var x = await _createAppneuronProductCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task AppneuronProduct_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateAppneuronProductCommand
            {
                ProductName = "test"
            };

            _appneuronProductRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .ReturnsAsync(new AppneuronProduct
                {
                    ProductName = "Test",
                    Id = new ObjectId("507f191e810c19729de860ea")
                });

            _appneuronProductRepository.Setup(x => x.UpdateAsync(It.IsAny<AppneuronProduct>(),
                It.IsAny<Expression<Func<AppneuronProduct, bool>>>()));

            var x = await _updateAppneuronProductCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task AppneuronProduct_UpdateCommand_AppneuronProductNotFound()
        {
            //Arrange
            var command = new UpdateAppneuronProductCommand
            {
                ProductName = "test_name"
            };

            _appneuronProductRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .Returns(Task.FromResult<AppneuronProduct>(null));

            _appneuronProductRepository.Setup(x =>
                x.Update(It.IsAny<AppneuronProduct>(), It.IsAny<Expression<Func<AppneuronProduct, bool>>>()));
            var x = await _updateAppneuronProductCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.AppneuronProductNotFound);
        }

        [Test]
        public async Task AppneuronProduct_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteAppneuronProductCommand();

            _appneuronProductRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .ReturnsAsync(new AppneuronProduct
                {
                    ProductName = "Test",
                    Id = new ObjectId("507f191e810c19729de860ea")
                });

            _appneuronProductRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<AppneuronProduct>(), It.IsAny<Expression<Func<AppneuronProduct, bool>>>()));

            var x = await _deleteAppneuronProductCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task AppneuronProduct_DeleteCommand_AppneuronProductNotFound()
        {
            //Arrange
            var command = new DeleteAppneuronProductCommand();

            _appneuronProductRepository.Setup(x
                    => x.GetAsync(It.IsAny<Expression<Func<AppneuronProduct, bool>>>()))
                .Returns(Task.FromResult<AppneuronProduct>(null));

            _appneuronProductRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<AppneuronProduct>(), It.IsAny<Expression<Func<AppneuronProduct, bool>>>()));

            var x = await _deleteAppneuronProductCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.AppneuronProductNotFound);
        }
    }
}