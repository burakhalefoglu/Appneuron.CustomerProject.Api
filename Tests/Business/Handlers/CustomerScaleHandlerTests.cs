
using Business.Handlers.CustomerScales.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.CustomerScales.Queries.GetCustomerScaleQuery;
using Entities.Concrete;
using static Business.Handlers.CustomerScales.Queries.GetCustomerScalesQuery;
using static Business.Handlers.CustomerScales.Commands.CreateCustomerScaleCommand;
using Business.Handlers.CustomerScales.Commands;
using Business.Constants;
using static Business.Handlers.CustomerScales.Commands.UpdateCustomerScaleCommand;
using static Business.Handlers.CustomerScales.Commands.DeleteCustomerScaleCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class CustomerScaleHandlerTests
    {
        private Mock<ICustomerScaleRepository> _customerScaleRepository;
        private Mock<IMediator> _mediator;

        private GetCustomerScaleQueryHandler _getCustomerScaleQueryHandler;
        private GetCustomerScalesQueryHandler _getCustomerScalesQueryHandler;
        private CreateCustomerScaleCommandHandler _createCustomerScaleCommandHandler;
        private UpdateCustomerScaleCommandHandler _updateCustomerScaleCommandHandler;
        private DeleteCustomerScaleCommandHandler _deleteCustomerScaleCommandHandler;


        [SetUp]
        public void Setup()
        {
            _customerScaleRepository = new Mock<ICustomerScaleRepository>();
            _mediator = new Mock<IMediator>();

            _getCustomerScaleQueryHandler = new GetCustomerScaleQueryHandler(_customerScaleRepository.Object, _mediator.Object);
            _getCustomerScalesQueryHandler = new GetCustomerScalesQueryHandler(_customerScaleRepository.Object, _mediator.Object);
            _createCustomerScaleCommandHandler = new CreateCustomerScaleCommandHandler(_customerScaleRepository.Object, _mediator.Object);
            _updateCustomerScaleCommandHandler = new UpdateCustomerScaleCommandHandler(_customerScaleRepository.Object, _mediator.Object);
            _deleteCustomerScaleCommandHandler = new DeleteCustomerScaleCommandHandler(_customerScaleRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task CustomerScale_GetQuery_Success()
        {
            //Arrange
            var query = new GetCustomerScaleQuery();

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>())).ReturnsAsync(new CustomerScale()
            {
                Id = 1,
                Name = "Test"
            });

            //Act
            var x = await _getCustomerScaleQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.Name.Should().Be("Test");

        }

        [Test]
        public async Task CustomerScale_GetQueries_Success()
        {
            //Arrange
            var query = new GetCustomerScalesQuery();

            _customerScaleRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync(new List<CustomerScale> { 
                        new()
                        {
                            Id = 1,
                            Name = "Test1"
                        },
                        
                        new()
                        {
                            Id = 2,
                            Name = "Test2"
                        },

                        });

            //Act
            var x = await _getCustomerScalesQueryHandler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task CustomerScale_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateCustomerScaleCommand
            {
                Name = "Test",
                Description = "TestDescription"
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync((CustomerScale)null);

            _customerScaleRepository.Setup(x => x.Add(It.IsAny<CustomerScale>())).Returns(new CustomerScale());

            var x = await _createCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerScaleRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task CustomerScale_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateCustomerScaleCommand
            {
                Name = "Test",
                Description = "TestDescription"
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                .ReturnsAsync(new CustomerScale());

            _customerScaleRepository.Setup(x => x.Add(It.IsAny<CustomerScale>())).Returns(new CustomerScale());

            var x = await _createCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task CustomerScale_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateCustomerScaleCommand
            {
                Id = 1,
                Description = "TestDescription",
                Name = "Test"
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync(new CustomerScale());

            _customerScaleRepository.Setup(x => x.Update(It.IsAny<CustomerScale>())).Returns(new CustomerScale());

            var x = await _updateCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerScaleRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task CustomerScale_UpdateCommand_CustomerScaleNotFound()
        {
            //Arrange
            var command = new UpdateCustomerScaleCommand
            {
                Id = 1,
                Description = "TestDescription",
                Name = "Test"
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync((CustomerScale)null);

            _customerScaleRepository.Setup(x => x.Update(It.IsAny<CustomerScale>())).Returns(new CustomerScale());

            var x = await _updateCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerScaleNotFound);
        }

        [Test]
        public async Task CustomerScale_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteCustomerScaleCommand
            {
                Id = 1
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync(new CustomerScale());

            _customerScaleRepository.Setup(x => x.Delete(It.IsAny<CustomerScale>()));

            var x = await _deleteCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            _customerScaleRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
        
        [Test]
        public async Task CustomerScale_DeleteCommand_CustomerScaleNotFound()
        {
            //Arrange
            var command = new DeleteCustomerScaleCommand
            {
                Id = 1
            };

            _customerScaleRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<CustomerScale, bool>>>()))
                        .ReturnsAsync((CustomerScale)null);

            _customerScaleRepository.Setup(x => 
                x.Delete(It.IsAny<CustomerScale>()));

            var x = await _deleteCustomerScaleCommandHandler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.CustomerScaleNotFound);
        }
    }
}

