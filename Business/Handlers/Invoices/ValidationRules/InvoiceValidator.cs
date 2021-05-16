using Business.Handlers.Invoices.Commands;
using FluentValidation;

namespace Business.Handlers.Invoices.ValidationRules
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(x => x.BillNo).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.LastPaymentTime).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.UnitPrice).NotEmpty();
            RuleFor(x => x.IsItPaid).NotEmpty();
        }
    }

    public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        public UpdateInvoiceValidator()
        {
            RuleFor(x => x.BillNo).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.LastPaymentTime).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.UnitPrice).NotEmpty();
            RuleFor(x => x.IsItPaid).NotEmpty();
        }
    }
}