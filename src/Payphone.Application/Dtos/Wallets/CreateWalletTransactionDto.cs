using Payphone.Domain.Enums;

namespace Payphone.Application.Dtos.Wallets;

public class CreateWalletTransactionDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public int? ToWalletId { get; set; }
}