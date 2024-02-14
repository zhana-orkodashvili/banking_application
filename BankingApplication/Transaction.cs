namespace Banking_Application;

public class Transaction
{
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public decimal AmountGEL { get; set; }
    public decimal AmountUSD { get; set; }
    public decimal AmountEUR { get; set; }
}