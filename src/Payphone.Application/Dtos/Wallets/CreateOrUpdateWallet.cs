namespace Payphone.Application.Dtos.Wallets;

public class CreateOrUpdateWallet
{
    public string? OwnerDocumentId { get; set; }
    public string? OwnerName { get; set; }
    public decimal CurrentBalance { get; set; }
}