
using Business.Handlers.GamePlatforms.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.GamePlatforms.Queries.GetGamePlatformQuery;
using Entities.Concrete;
using static Business.Handlers.GamePlatforms.Queries.GetGamePlatformsQuery;
using static Business.Handlers.GamePlatforms.Commands.CreateGamePlatformCommand;
using Business.Handlers.GamePlatforms.Commands;
using Business.Constants;
using static Business.Handlers.GamePlatforms.Commands.UpdateGamePlatformCommand;
using static Business.Handlers.GamePlatforms.Commands.DeleteGamePlatformCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class GamePlatformHandlerTests
    {
        private Mock<IGamePlatformRepository> _gamePlatformRepository;
        private Mock<IMediator> _mediator;

        private GetGamePlatformQueryHandler _getGamePlatformQueryHandler;
        private GetGamePlatformsQueryHandler _getGamePlatformsQueryHandler;
        private CreateGamePlatformCommandHandler _createGamePlatformCommandHandler;
        private UpdateGamePlatformCommandHandler _updateGamePlatformCommandHandler;
        private DeleteGamePlatformCommandHandler _deleteGamePlatformCommandHandler;


        [SetUp]
        public void Setup()
        {
            _gamePlatformRepository = new Mock<IGamePlatformRepository>();
            _mediator = new Mock<IMediator>();

            _getGamePlatformQueryHandler = new GetGamePlatformQueryHandler(_gamePlatformRepository.Object, _mediator.Object);
            _getGamePlatformsQueryHandler = new GetGamePlatformsQueryHandler(_gamePlatformRepository.Object, _mediator.Object);
            _createGamePlatformCommandHandler = new CreateGamePlatformCommandHandler(_gamePlatformRepository.Object, _mediator.Object);
            _updateGamePlatformCommandHandler = new UpdateGamePlatformCommandHandler(_gamePlatformRepository.Object, _mediator.Object);
            _deleteGamePlatformCommandHandler = new DeleteGamePlatformCommandHandler(_gamePlatformRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task GamePlatform_GetQuery_Success()
        {
            //Arrange
            var query = new GetGamePlatformQuery
            {
                Id = 1
            };

            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>())).ReturnsAsync(new GamePlatform()
            {
                Id = 1,
                PlatformName = "Test"
            });

            //Act
            var x = await _getGamePlatformQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);

        }

        [Test]
        public async Task GamePlatform_GetQueries_Success()
        {
            //Arrange
            var query = new GetGamePlatformsQuery();
            
            _gamePlatformRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync(new List<GamePlatform>
                        {
                            new()
                            {
                                Id = 1,
                                PlatformDescription = "TestDesc"
                            },
                            
                            new()
                            {
                                Id = 2,
                                PlatformDescription = "TestDesc2"
                            }
                        });


            //Act
            var x = await _getGamePlatformsQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task GamePlatform_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateGamePlatformCommand
            {
                PlatformName = "Test",
                PlatformDescription = "TestDesc"
            };


            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync((GamePlatform)null);

            _gamePlatformRepository.Setup(x => x.Add(It.IsAny<GamePlatform>())).Returns(new GamePlatform());

            var x = await _createGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _gamePlatformRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task GamePlatform_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateGamePlatformCommand
            {
                PlatformName = "Test",
                PlatformDescription = "TestDesc"
            };


            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                .ReturnsAsync(new GamePlatform());

            _gamePlatformRepository.Setup(x => x.Add(It.IsAny<GamePlatform>())).Returns(new GamePlatform());

            var x = await _createGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }


        [Test]
        public async Task GamePlatform_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateGamePlatformCommand
            {
                PlatformName = "test",
                Id = 1
            };


            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync(new GamePlatform()
                        {
                            Id = 1
                        });

            _gamePlatformRepository.Setup(x => x.Update(It.IsAny<GamePlatform>())).Returns(new GamePlatform());

            var x = await _updateGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _gamePlatformRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task GamePlatform_UpdateCommand_GamePlatformNotFound()
        {
            //Arrange
            var command = new UpdateGamePlatformCommand
            {
                PlatformName = "test",
                Id = 1
            };


            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync((GamePlatform)null);

            _gamePlatformRepository.Setup(x => x.Update(It.IsAny<GamePlatform>())).Returns(new GamePlatform());

            var x = await _updateGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.GamePlatformNotFound);
        }

        [Test]
        public async Task GamePlatform_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteGamePlatformCommand
            {
                Id = 1
            };

            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync(new GamePlatform());

            _gamePlatformRepository.Setup(x => x.Delete(It.IsAny<GamePlatform>()));

            var x = await _deleteGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _gamePlatformRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }


        [Test]
        public async Task GamePlatform_DeleteCommand_GamePlatformNotFound()
        {
            //Arrange
            var command = new DeleteGamePlatformCommand
            {
                Id = 1
            };

            _gamePlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<GamePlatform, bool>>>()))
                        .ReturnsAsync((GamePlatform)null);

            _gamePlatformRepository.Setup(x => x.Delete(It.IsAny<GamePlatform>()));

            var x = await _deleteGamePlatformCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.GamePlatformNotFound);
        }


    }
}

