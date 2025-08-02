using Payphone.Application.Extensions;
using Payphone.Domain.Enums;

namespace Payphone.Application.Dtos.Wallets;

public class WalletTransactionDto : BaseGet
{
    public int FromWalletId { get; set; }

    public int ToWalletId { get; set; }

    public decimal CurrentWalletBalance { get; set; }

    public decimal Amount { get; set; }
    [JsonIgnore]
    public TransactionType Type { get; set; }

    public string? TypeDescription => Type.GetDisplay();
    
}