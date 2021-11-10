using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Industries.Commands;
using Business.Handlers.Industries.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Industries.Queries.GetIndustryQuery;
using static Business.Handlers.Industries.Queries.GetIndustriesQuery;
using static Business.Handlers.Industries.Commands.CreateIndustryCommand;
using static Business.Handlers.Industries.Commands.UpdateIndustryCommand;
using static Business.Handlers.Industries.Commands.DeleteIndustryCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class IndustryHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _industryRepository = new Mock<IIndustryRepository>();
            _mediator = new Mock<IMediator>();

            _getIndustryQueryHandler = new GetIndustryQueryHandler(_industryRepository.Object, _mediator.Object);
            _getIndustriesQueryHandler = new GetIndustriesQueryHandler(_industryRepository.Object, _mediator.Object);
            _createIndustryCommandHandler =
                new CreateIndustryCommandHandler(_industryRepository.Object, _mediator.Object);
            _updateIndustryCommandHandler =
                new UpdateIndustryCommandHandler(_industryRepository.Object, _mediator.Object);
            _deleteIndustryCommandHandler =
                new DeleteIndustryCommandHandler(_industryRepository.Object, _mediator.Object);
        }

        private Mock<IIndustryRepository> _industryRepository;
        private Mock<IMediator> _mediator;

        private GetIndustryQueryHandler _getIndustryQueryHandler;
        private GetIndustriesQueryHandler _getIndustriesQueryHandler;
        private CreateIndustryCommandHandler _createIndustryCommandHandler;
        private UpdateIndustryCommandHandler _updateIndustryCommandHandler;
        private DeleteIndustryCommandHandler _deleteIndustryCommandHandler;

        [Test]
        public async Task Industry_GetQuery_Success()
        {
            //Arrange
            var query = new GetIndustryQuery
            {
                Id = 1
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>())).ReturnsAsync(
                new Industry
                {
                    Id = 1,
                    Name = "Test"
                });

            //Act
            var x = await _getIndustryQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Id.Should().Be(1);
        }

        [Test]
        public async Task Industry_GetQueries_Success()
        {
            //Arrange
            var query = new GetIndustriesQuery();

            _industryRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync(new List<Industry>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Test"
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Test"
                    }
                });

            //Act
            var x = await _getIndustriesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Industry>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Industry_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateIndustryCommand
            {
                Name = "Test"
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync((Industry)null);

            _industryRepository.Setup(x => x.Add(It.IsAny<Industry>())).Returns(new Industry());

            var x = await _createIndustryCommandHandler.Handle(command, new CancellationToken());

            _industryRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Industry_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateIndustryCommand
            {
                Name = "Test"
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync(new Industry());

            _industryRepository.Setup(x => x.Add(It.IsAny<Industry>())).Returns(new Industry());

            var x = await _createIndustryCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Industry_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateIndustryCommand
            {
                Name = "test",
                Id = 1
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync(new Industry());

            _industryRepository.Setup(x => x.Update(It.IsAny<Industry>())).Returns(new Industry());

            var x = await _updateIndustryCommandHandler.Handle(command, new CancellationToken());

            _industryRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Industry_UpdateCommand_IndustryNotFound()
        {
            //Arrange
            var command = new UpdateIndustryCommand
            {
                Name = "test",
                Id = 1
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync((Industry)null);

            _industryRepository.Setup(x => x.Update(It.IsAny<Industry>())).Returns(new Industry());

            var x = await _updateIndustryCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.IndustryNotFound);
        }

        [Test]
        public async Task Industry_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteIndustryCommand
            {
                Id = 1
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync(new Industry());

            _industryRepository.Setup(x => x.Delete(It.IsAny<Industry>()));

            var x = await _deleteIndustryCommandHandler.Handle(command, new CancellationToken());

            _industryRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Industry_DeleteCommand_IndustryNotFound()
        {
            //Arrange
            var command = new DeleteIndustryCommand
            {
                Id = 1
            };

            _industryRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Industry, bool>>>()))
                .ReturnsAsync((Industry)null);

            _industryRepository.Setup(x => x.Delete(It.IsAny<Industry>()));

            var x = await _deleteIndustryCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.IndustryNotFound);
        }
    }
}