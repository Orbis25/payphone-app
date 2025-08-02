using FluentValidation;
using Payphone.Application.Dtos.Wallets;

namespace Payphone.Application.Validators;

public class CreateOrUpdateWalletValidator : AbstractValidator<CreateOrUpdateWallet>
{
    public CreateOrUpdateWalletValidator()
    {
        RuleFor(x => x.OwnerDocumentId).NotNull()
            .WithMessage("field OwnerDocumentId is required")
            .NotEmpty().WithMessage("field OwnerDocumentId is required")
            .MaximumLength(20).WithMessage("field OwnerDocumentId must be less than or equal to 20 characters")
            .MinimumLength(8).WithMessage("field OwnerDocumentId must be greater than or equal to 8 characters")
            .Matches(@"^\d+$").WithMessage("field OwnerDocumentId must contain only numbers");

        RuleFor(x => x.OwnerName).NotNull()
            .WithMessage("field OwnerName is required")
            .NotEmpty().WithMessage("field OwnerName is required")
            .MaximumLength(100).WithMessage("field OwnerName must be less than or equal to 100 characters")
            .MinimumLength(3).WithMessage("field OwnerName must be greater than or equal to 3 characters");
        
        RuleFor(x => x.CurrentBalance).NotNull()
            .WithMessage("field CurrentBalance is required")
            .NotEmpty().WithMessage("field CurrentBalance is required")
            .GreaterThanOrEqualTo(1).WithMessage("The initial balance must be greater than or equal to 1");
    }
}