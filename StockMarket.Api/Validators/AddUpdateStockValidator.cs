using FluentValidation;
using StockMarket.Core.Models;

namespace StockMarket.Api.Validators
{
    public class AddUpdateStockValidator : AbstractValidator<Stock>
    {
        public AddUpdateStockValidator()
        {
            RuleFor(s => s.Code)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(s => s.Name)
                .NotEmpty();
            RuleFor(s => s.Price)
                .GreaterThan(0);
            RuleFor(s => s.Exchange)
                .NotEmpty();
        }
    }
}
