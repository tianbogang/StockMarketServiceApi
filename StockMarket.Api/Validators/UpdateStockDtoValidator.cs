using FluentValidation;
using StockMarket.Core.Dtos;

namespace StockMarket.Api.Validators
{
    public class UpdateStockDtoValidator : AbstractValidator<StockDto>
    {
        public UpdateStockDtoValidator()
        {
            RuleFor(s => s.Code)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(s => s.Price)
                .GreaterThan(0);
        }
    }
}
