using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Invoices.Commands;
using Business.Handlers.Invoices.Queries;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using static Business.Handlers.Invoices.Queries.GetInvoiceQuery;
using static Business.Handlers.Invoices.Queries.GetInvoicesQuery;
using static Business.Handlers.Invoices.Commands.CreateInvoiceCommand;
using static Business.Handlers.Invoices.Commands.UpdateInvoiceCommand;
using static Business.Handlers.Invoices.Commands.DeleteInvoiceCommand;


namespace Tests.Business.Handlers
{
    [TestFixture]
    public class InvoiceHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _invoiceRepository = new Mock<IInvoiceRepository>();
            _mediator = new Mock<IMediator>();

            _getInvoiceQueryHandler = new GetInvoiceQueryHandler(_invoiceRepository.Object);
            _getInvoicesQueryHandler = new GetInvoicesQueryHandler(_invoiceRepository.Object, _mediator.Object);
            _getCreateInvoiceCommandHandler =
                new CreateInvoiceCommandHandler(_invoiceRepository.Object, _mediator.Object);
            _updateInvoiceCommandHandler = new UpdateInvoiceCommandHandler(_invoiceRepository.Object);
            _deleteInvoiceCommandHandler = new DeleteInvoiceCommandHandler(_invoiceRepository.Object);
        }

        private Mock<IInvoiceRepository> _invoiceRepository;
        private Mock<IMediator> _mediator;

        private GetInvoiceQueryHandler _getInvoiceQueryHandler;
        private GetInvoicesQueryHandler _getInvoicesQueryHandler;
        private CreateInvoiceCommandHandler _getCreateInvoiceCommandHandler;
        private UpdateInvoiceCommandHandler _updateInvoiceCommandHandler;
        private DeleteInvoiceCommandHandler _deleteInvoiceCommandHandler;

        [Test]
        public async Task Invoice_GetQuery_Success()
        {
            //Arrange
            var query = new GetInvoiceQuery
            {
                Id = 1
            };

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>())).ReturnsAsync(
                new Invoice
                {
                    Id = 1,
                    UserId = 12
                });

            //Act
            var x = await _getInvoiceQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.UserId.Should().Be(12);
        }

        [Test]
        public async Task Invoice_GetQueries_Success()
        {
            //Arrange
            var query = new GetInvoicesQuery();

            _invoiceRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(new List<Invoice>
                {
                    new()
                    {
                        Id = 1,
                        UserId = 12
                    },

                    new()
                    {
                        Id = 2,
                        UserId = 21
                    }
                }.AsQueryable());

            //Act
            var x = await _getInvoicesQueryHandler.Handle(query, new CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            x.Data.ToList().Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Invoice_CreateCommand_Success()
        {
            //Arrange
            var command = new CreateInvoiceCommand
            {
                UserId = 1,
                BillNo = "AB252D"
            };

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync((Invoice) null);

            _invoiceRepository.Setup(x => x.AddAsync(It.IsAny<Invoice>()));

            var x = await _getCreateInvoiceCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Invoice_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateInvoiceCommand
            {
                UserId = 1,
                BillNo = "AB252D"
            };

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(new Invoice());

            _invoiceRepository.Setup(x => x.Add(It.IsAny<Invoice>()));

            var x = await _getCreateInvoiceCommandHandler.Handle(command, new CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }


        [Test]
        public async Task Invoice_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateInvoiceCommand
            {
                Id = 1
            };

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(new Invoice());

            _invoiceRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Invoice>()));

            var x = await _updateInvoiceCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Invoice_UpdateCommand_InvoiceNotFound()
        {
            //Arrange
            var command = new UpdateInvoiceCommand
            {
                Id = 1
            };

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync((Invoice) null);

            _invoiceRepository.Setup(x => x.Update(It.IsAny<Invoice>()));

            var x = await _updateInvoiceCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.InvoiceNotFound);
        }


        [Test]
        public async Task Invoice_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteInvoiceCommand();

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(new Invoice());

            _invoiceRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Invoice>()));

            var x = await _deleteInvoiceCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task Invoice_DeleteCommand_InvoiceNotFound()
        {
            //Arrange
            var command = new DeleteInvoiceCommand();

            _invoiceRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync((Invoice) null);

            _invoiceRepository.Setup(x =>
                x.UpdateAsync(It.IsAny<Invoice>()));

            var x = await _deleteInvoiceCommandHandler.Handle(command, new CancellationToken());
            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.InvoiceNotFound);
        }
    }
}