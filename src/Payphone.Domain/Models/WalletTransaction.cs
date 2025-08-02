using Payphone.Domain.Core.Models;
using Payphone.Domain.Enums;

namespace Payphone.Domain.Models;

public class WalletTransaction : BaseModel
{
    public int FromWalletId { get; set; }
    
    public Wallet? FromWallet { get; set; }
    
    public int? ToWalletId { get; set; }

    public Wallet? ToWallet { get; set; }
    public decimal Amount { get; set; }
    public decimal CurrentWalletBalance { get; set; }
    public TransactionType Type { get; set; }
}