using Payphone.Domain.Core.Models;

namespace Payphone.Domain.Models;

public class Wallet : BaseModel
{
    public Guid WalletCode { get; set; } = Guid.NewGuid();
    public string? OwnerDocumentId { get; set; }
    public decimal OwnerName { get; set; }
    public decimal CurrentBalance { get; set; }
    public ICollection<WalletTransaction>? Transactions { get; set; }
}