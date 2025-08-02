using Payphone.Domain.Core.Models;
using Payphone.Domain.Enums;

namespace Payphone.Domain.Models;

public class WalletTransaction : BaseModel
{
    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}