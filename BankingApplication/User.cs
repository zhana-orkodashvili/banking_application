namespace Banking_Application;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Card CardDetails { get; set; }
    public List<Transaction> TransactionHistory { get; set; }

    
    public void CheckBalance()
    {
        Console.WriteLine("Account Balance:");
        var latestTransaction = TransactionHistory[0];
        
        Console.WriteLine($"GEL: {latestTransaction.AmountGEL}");
        Console.WriteLine($"EUR: {latestTransaction.AmountEUR}");
        Console.WriteLine($"USD: {latestTransaction.AmountUSD}");
    }
    
    public void MakeWithdrawal(decimal amount)
    {
        var withdrawalTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Withdrawal",
            AmountGEL = TransactionHistory[0].AmountGEL-amount,
            AmountUSD = TransactionHistory[0].AmountUSD,
            AmountEUR = TransactionHistory[0].AmountEUR
            
        };

        TransactionHistory.Insert(0,withdrawalTransaction);
        Console.WriteLine($"Withdrawn {amount} GEL successfully. \nCurrent Balance: {TransactionHistory[0].AmountGEL} GEL");
    }
    
    public void GetLastTransactions()
    {
        const int maxTransactionsToShow = 5;

        var transactionsToShow = Math.Min(maxTransactionsToShow, TransactionHistory.Count);

        if (transactionsToShow == 0)
        {
            Console.WriteLine("No transactions available.");
            return;
        }

        Console.WriteLine($"Showing {transactionsToShow} latest transactions:");

        for (var i = 0; i < transactionsToShow; i++)
        {
            var transaction = TransactionHistory[i];
            Console.WriteLine($"Transaction Date: {transaction.TransactionDate}");
            Console.WriteLine($"Transaction Type: {transaction.TransactionType}");
            Console.WriteLine($"Amount in GEL: {transaction.AmountGEL}");
            Console.WriteLine($"Amount in EUR: {transaction.AmountEUR}");
            Console.WriteLine($"Amount in USD: {transaction.AmountUSD}");
            Console.WriteLine();
        }
    }
    
    public void MakeDeposit(decimal amount)
    {
        var depositTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Fill Amount",
            AmountGEL = TransactionHistory[0].AmountGEL+amount,
            AmountUSD = TransactionHistory[0].AmountUSD,
            AmountEUR = TransactionHistory[0].AmountEUR
            
        };

        TransactionHistory.Insert(0,depositTransaction);
        Console.WriteLine($"Added {amount} GEL successfully. \nCurrent Balance: {TransactionHistory[0].AmountGEL} GEL");
    }
    
    public void ChangePin(string newPin)
    {
        CardDetails.PinCode = newPin;
        Console.WriteLine("PIN changed successfully.");
    }
    
    public void CurrencyConversion(decimal amount, string toCurrency)
    {
        
        const decimal gelToUsdRate = 0.377m; 
        const decimal gelToEurRate = 0.35m; 

        var balanceUsd = TransactionHistory[0].AmountUSD;
        var balanceGel = TransactionHistory[0].AmountGEL;
        var balanceEur = TransactionHistory[0].AmountEUR;
        

        decimal convertedAmount = 0;
        
        Transaction? conversionTransaction;
        switch (toCurrency.ToLower())
        {
            case "usd":
                convertedAmount = amount * gelToUsdRate;
                
                conversionTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Fill Amount",
                    AmountGEL = balanceGel-amount,
                    AmountUSD = balanceUsd + convertedAmount,
                    AmountEUR = balanceEur
            
                };

                TransactionHistory.Insert(0,conversionTransaction);
                Console.WriteLine($"Added {convertedAmount} USD successfully.");
                break;
            
            case "eur":
                convertedAmount = amount * gelToEurRate;
                conversionTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Fill Amount",
                    AmountGEL = balanceGel-amount,
                    AmountUSD = balanceUsd,
                    AmountEUR = balanceEur+convertedAmount
            
                };

                TransactionHistory.Insert(0,conversionTransaction);
                Console.WriteLine($"Added {convertedAmount} EUR successfully.");
                break;
            default:
                Console.WriteLine("Invalid currency for conversion. Can only convert to USD or EUR.");
                break;
        }

      
    }
    
    
    
}