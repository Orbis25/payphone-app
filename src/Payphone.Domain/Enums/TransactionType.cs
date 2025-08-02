using System.ComponentModel.DataAnnotations;

namespace Payphone.Domain.Enums;

public enum TransactionType
{
    [Display(Name = "Credit")]
    Credit,
    [Display(Name = "Debit")]
    Debit,
}