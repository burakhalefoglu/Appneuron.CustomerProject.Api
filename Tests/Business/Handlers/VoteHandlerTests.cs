using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Votes.Commands;
using Business.Handlers.Votes.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Votes.Queries.GetVoteQuery;
using static Business.Handlers.Votes.Queries.GetVotesQuery;
using static Business.Handlers.Votes.Commands.CreateVoteCommand;
using static Business.Handlers.Votes.Commands.UpdateVoteCommand;
using static Business.Handlers.Votes.Commands.DeleteVoteCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class VoteHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _voteRepository = new Mock<IVoteRepository>();
            _mediator = new Mock<IMediator>();

            _getVoteQueryHandler = new GetVoteQueryHandler(_voteRepository.Object, _mediator.Object);
            _getVotesQueryHandler = new GetVotesQueryHandler(_voteRepository.Object, _mediator.Object);
            _createVoteCommandHandler = new CreateVoteCommandHandler(_voteRepository.Object, _mediator.Object);
            _updateVoteCommandHandler = new UpdateVoteCommandHandler(_voteRepository.Object, _mediator.Object);
            _deleteVoteCommandHandler = new DeleteVoteCommandHandler(_voteRepository.Object, _mediator.Object);
        }

        private Mock<IVoteRepository> _voteRepository;
        private Mock<IMediator> _mediator;

        private GetVoteQueryHandler _getVoteQueryHandler;
        private GetVotesQueryHandler _getVotesQueryHandler;
        private CreateVoteCommandHandler _createVoteCommandHandler;
        private UpdateVoteCommandHandler _updateVoteCommandHandler;
        private DeleteVoteCommandHandler _deleteVoteCommandHandler;

        [Test]
        public async Task Vote_GetQuery_Success()
        {
            //Arrange
            var query = new GetVoteQuery
            {
                Id = 1
            };

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>())).ReturnsAsync(new Vote
            {
                Id = 1,
                VoteName = "Test"
            });

            //Act
            var x = await _getVoteQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task Vote_GetQueries_Success()
        {
            //Arrange
            var query = new GetVotesQuery();

            _voteRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(new List<Vote>
                {
                    new()
                    {
                        Id = 1,
                        VoteName = "Test"
                    },
                    new()
                    {
                        Id = 2,
                        VoteName = "Test2"
                    }
                });


            //Act
            var x = await _getVotesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Vote_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateVoteCommand
            {
                VoteName = "Test",
                VoteValue = 1
            };

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync((Vote)null);

            _voteRepository.Setup(x => x.Add(It.IsAny<Vote>())).Returns(new Vote());

            var x = await _createVoteCommandHandler.Handle(command, new CancellationToken());

            _voteRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Vote_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateVoteCommand
            {
                VoteName = "Test",
                VoteValue = 1
            };

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(new Vote());

            _voteRepository.Setup(x => x.Add(It.IsAny<Vote>())).Returns(new Vote());

            var x = await _createVoteCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Vote_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateVoteCommand
            {
                VoteName = "test",
                Id = 1,
                VoteValue = 1
            };

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(new Vote());

            _voteRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns(new Vote());

            var x = await _updateVoteCommandHandler.Handle(command, new CancellationToken());

            _voteRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Vote_UpdateCommand_VoteNotFound()
        {
            //Arrange
            var command = new UpdateVoteCommand
            {
                VoteName = "test",
                Id = 1,
                VoteValue = 1
            };

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync((Vote)null);

            _voteRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns(new Vote());

            var x = await _updateVoteCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.VoteNotFound);
        }

        [Test]
        public async Task Vote_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteVoteCommand();

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(new Vote());

            _voteRepository.Setup(x => x.Delete(It.IsAny<Vote>()));

            var x = await _deleteVoteCommandHandler.Handle(command, new CancellationToken());

            _voteRepository.Verify(c => c.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Vote_DeleteCommand_VoteNotFound()
        {
            //Arrange
            var command = new DeleteVoteCommand();

            _voteRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync((Vote)null);

            _voteRepository.Setup(x => x.Delete(It.IsAny<Vote>()));

            var x = await _deleteVoteCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.VoteNotFound);
        }
    }
}