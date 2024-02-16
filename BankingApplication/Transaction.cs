namespace Banking_Application;

public class Transaction
{
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public decimal AmountGel { get; set; }
    public decimal AmountUsd { get; set; }
    public decimal AmountEur { get; set; }
}