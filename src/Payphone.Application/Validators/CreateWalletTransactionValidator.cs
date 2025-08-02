using FluentValidation;
using Payphone.Application.Dtos.Wallets;
using Payphone.Domain.Enums;

namespace Payphone.Application.Validators;

public class CreateWalletTransactionValidator : AbstractValidator<CreateWalletTransactionDto>
{
    public CreateWalletTransactionValidator()
    {
        RuleFor(x => x.Amount).NotNull()
            .WithMessage("field amount is required")
            .GreaterThanOrEqualTo(1).WithMessage("amount must be greater than or equal to 1");

        RuleFor(x => x.Type).NotNull()
            .WithMessage("Field transaction type is required")
            .IsInEnum().WithMessage("Field transaction type must be Credit or Debit");

        RuleFor(x => x.ToWalletId)
            .Must((dto, toWalletId) => dto.Type != TransactionType.Debit || toWalletId.HasValue)
            .GreaterThanOrEqualTo(1).WithMessage("ToWalletId is not valid")
            .WithMessage("ToWalletId is required when transaction type is Debit, if is a credit this transaction apply to same wallet");
    }
}