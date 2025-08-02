namespace Payphone.Application.Dtos.Wallets;

public class WalletDto : BaseGet
{
    public Guid WalletCode { get; set; }
    public string? OwnerDocumentId { get; set; }
    public string? OwnerName { get; set; }
    public decimal CurrentBalance { get; set; }
    public string? Currency => "USD";
}