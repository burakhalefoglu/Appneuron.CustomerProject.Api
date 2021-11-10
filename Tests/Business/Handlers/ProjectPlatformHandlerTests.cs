using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Fakes.Handlers.ProjectCounts;
using Business.Handlers.ProjectPlatforms.Commands;
using Business.Handlers.ProjectPlatforms.Queries;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.ProjectPlatforms.Queries.GetProjectPlatformQuery;
using static Business.Handlers.ProjectPlatforms.Queries.GetProjectPlatformsQuery;
using static Business.Handlers.ProjectPlatforms.Commands.CreateProjectPlatformCommand;
using static Business.Handlers.ProjectPlatforms.Commands.UpdateProjectPlatformCommand;
using static Business.Handlers.ProjectPlatforms.Commands.DeleteProjectPlatformCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class ProjectPlatformHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _projectPlatformRepository = new Mock<IProjectPlatformRepository>();
            _mediator = new Mock<IMediator>();

            _getProjectPlatformQueryHandler =
                new GetProjectPlatformQueryHandler(_projectPlatformRepository.Object, _mediator.Object);
            _getProjectPlatformsQueryHandler =
                new GetProjectPlatformsQueryHandler(_projectPlatformRepository.Object, _mediator.Object);
            _createProjectPlatformCommandHandler =
                new CreateProjectPlatformCommandHandler(_projectPlatformRepository.Object, _mediator.Object);
            _updateProjectPlatformCommandHandler =
                new UpdateProjectPlatformCommandHandler(_projectPlatformRepository.Object, _mediator.Object);
            _deleteProjectPlatformCommandHandler =
                new DeleteProjectPlatformCommandHandler(_projectPlatformRepository.Object, _mediator.Object);
        }

        private Mock<IProjectPlatformRepository> _projectPlatformRepository;
        private Mock<IMediator> _mediator;

        private GetProjectPlatformQueryHandler _getProjectPlatformQueryHandler;
        private GetProjectPlatformsQueryHandler _getProjectPlatformsQueryHandler;
        private CreateProjectPlatformCommandHandler _createProjectPlatformCommandHandler;
        private UpdateProjectPlatformCommandHandler _updateProjectPlatformCommandHandler;
        private DeleteProjectPlatformCommandHandler _deleteProjectPlatformCommandHandler;

        [Test]
        public async Task ProjectPlatform_GetQuery_Success()
        {
            //Arrange
            var query = new GetProjectPlatformQuery();

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));

            _projectPlatformRepository.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<ProjectPlatform, bool>>>())).ReturnsAsync(
                new List<ProjectPlatform>
                {
                    new()
                    {
                        Id = 1,
                        ProjectId = 12
                    },

                    new()
                    {
                        Id = 1,
                        ProjectId = 12
                    }
                });

            //Act
            var x = await _getProjectPlatformQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().Be(2);
        }

        [Test]
        public async Task ProjectPlatform_GetQuery_ProjectNotFound()
        {
            //Arrange
            var query = new GetProjectPlatformQuery();

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(0));

            _projectPlatformRepository.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<ProjectPlatform, bool>>>())).ReturnsAsync(
                new List<ProjectPlatform>
                {
                    new()
                    {
                        Id = 1,
                        ProjectId = 12
                    },

                    new()
                    {
                        Id = 1,
                        ProjectId = 12
                    }
                });

            //Act
            var x = await _getProjectPlatformQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeFalse();
        }

        [Test]
        public async Task ProjectPlatform_GetQueries_Success()
        {
            //Arrange
            var query = new GetProjectPlatformsQuery();

            _projectPlatformRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new List<ProjectPlatform>
                {
                    new()
                    {
                        GamePlatform = new GamePlatform(),
                        ProjectId = 22,
                        GamePlatformId = 1,
                        Project = new CustomerProject()
                    },
                    new()
                    {
                        GamePlatform = new GamePlatform(),
                        ProjectId = 23,
                        GamePlatformId = 1,
                        Project = new CustomerProject()
                    },
                    new()
                    {
                        GamePlatform = new GamePlatform(),
                        ProjectId = 12,
                        GamePlatformId = 3,
                        Project = new CustomerProject()
                    }
                });
            //Act
            var x = await _getProjectPlatformsQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task ProjectPlatform_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateProjectPlatformCommand
            {
                ProjectId = 12,
                GamePlatformId = 2
            };

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync((ProjectPlatform)null);

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));

            _projectPlatformRepository.Setup(x => x.Add(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _createProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            _projectPlatformRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task ProjectPlatform_CreateCommand_ProjectNotFound()
        {
            //Arrange
            var command = new CreateProjectPlatformCommand
            {
                ProjectId = 12,
                GamePlatformId = 2
            };

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync((ProjectPlatform)null);

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(0));

            _projectPlatformRepository.Setup(x => x.Add(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _createProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task ProjectPlatform_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateProjectPlatformCommand
            {
                ProjectId = 12,
                GamePlatformId = 2
            };

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new ProjectPlatform());

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));

            _projectPlatformRepository.Setup(x => x.Add(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _createProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task ProjectPlatform_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateProjectPlatformCommand();
            //command.ProjectPlatformName = "test";

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new ProjectPlatform());

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));

            _projectPlatformRepository.Setup(x => x.Update(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _updateProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            _projectPlatformRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task ProjectPlatform_UpdateCommand_ProjectNotFound()
        {
            //Arrange
            var command = new UpdateProjectPlatformCommand();
            //command.ProjectPlatformName = "test";

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new ProjectPlatform());

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(0));

            _projectPlatformRepository.Setup(x => x.Update(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _updateProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task ProjectPlatform_UpdateCommand_ProjectPlatformNotFound()
        {
            //Arrange
            var command = new UpdateProjectPlatformCommand();
            //command.ProjectPlatformName = "test";

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync((ProjectPlatform)null);

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));

            _projectPlatformRepository.Setup(x => x.Update(It.IsAny<ProjectPlatform>())).Returns(new ProjectPlatform());

            var x = await _updateProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectPlatformNotFound);
        }

        [Test]
        public async Task ProjectPlatform_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteProjectPlatformCommand();

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new ProjectPlatform());

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));


            _projectPlatformRepository.Setup(x => x.Delete(It.IsAny<ProjectPlatform>()));

            var x = await _deleteProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            _projectPlatformRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task ProjectPlatform_DeleteCommand_ProjectNotFound()
        {
            //Arrange
            var command = new DeleteProjectPlatformCommand();

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync(new ProjectPlatform());

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(0));


            _projectPlatformRepository.Setup(x => x.Delete(It.IsAny<ProjectPlatform>()));

            var x = await _deleteProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectNotFound);
        }

        [Test]
        public async Task ProjectPlatform_DeleteCommand_ProjectPlatformNotFound()
        {
            //Arrange
            var command = new DeleteProjectPlatformCommand();

            _projectPlatformRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<ProjectPlatform, bool>>>()))
                .ReturnsAsync((ProjectPlatform)null);

            _mediator.Setup(x => x.Send(It.IsAny<GetProjectCountInternalQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<int>(2));


            _projectPlatformRepository.Setup(x => x.Delete(It.IsAny<ProjectPlatform>()));

            var x = await _deleteProjectPlatformCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.ProjectPlatformNotFound);
        }
    }
}