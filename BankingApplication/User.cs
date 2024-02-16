namespace Banking_Application;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Card CardDetails { get; set; }
    public List<Transaction> TransactionHistory { get; set; }

    public User(string firstName, string lastName, Card cardDetails, List<Transaction> transactionHistory)
    {
        FirstName = firstName;
        LastName = lastName;
        CardDetails = cardDetails;
        TransactionHistory = transactionHistory;
    }
    
    public void CheckBalance()
    {
        if (TransactionHistory.Count == 0)
        {
            Console.WriteLine("No Transactions Were Made.");
            return;
        }

        var latestTransaction = TransactionHistory[0];
        Console.WriteLine("Account Balance:");
        Console.WriteLine($"GEL: {latestTransaction.AmountGel}");
        Console.WriteLine($"EUR: {latestTransaction.AmountEur}");
        Console.WriteLine($"USD: {latestTransaction.AmountUsd}");
        Console.WriteLine();
        
        var checkBalanceTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Check Balance",
            AmountGel = latestTransaction.AmountGel,
            AmountUsd = latestTransaction.AmountUsd,
            AmountEur = latestTransaction.AmountEur
            
        };
        TransactionHistory.Insert(0,checkBalanceTransaction);
    }
    
    public void MakeWithdrawal(decimal amount)
    {
        var withdrawalTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Withdrawal",
            AmountGel = TransactionHistory[0].AmountGel-amount,
            AmountUsd = TransactionHistory[0].AmountUsd,
            AmountEur = TransactionHistory[0].AmountEur
            
        };

        TransactionHistory.Insert(0,withdrawalTransaction);
        Console.WriteLine($"Withdrawn {amount}GEL successfully. \nCurrent Balance: {TransactionHistory[0].AmountGel}GEL");
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
            Console.WriteLine($"Amount in GEL: {transaction.AmountGel}");
            Console.WriteLine($"Amount in EUR: {transaction.AmountEur}");
            Console.WriteLine($"Amount in USD: {transaction.AmountUsd}");
            Console.WriteLine();
        }
    }
    
    public void MakeDeposit(decimal amount)
    {
        var depositTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Fill Amount",
            AmountGel = TransactionHistory[0].AmountGel+amount,
            AmountUsd = TransactionHistory[0].AmountUsd,
            AmountEur = TransactionHistory[0].AmountEur
            
        };

        TransactionHistory.Insert(0,depositTransaction);
        Console.WriteLine($"Added {amount}GEL successfully. \nCurrent Balance: {TransactionHistory[0].AmountGel}GEL");
    }
    
    public void ChangePin(string newPin)
    {
        CardDetails.PinCode = newPin;
        var changePinTransaction = new Transaction
        {
            TransactionDate = DateTime.Now,
            TransactionType = "Change PIN",
            AmountGel = TransactionHistory[0].AmountGel,
            AmountUsd = TransactionHistory[0].AmountUsd,
            AmountEur = TransactionHistory[0].AmountEur
            
        };

        TransactionHistory.Insert(0,changePinTransaction);
        Console.WriteLine("PIN changed successfully.");
    }
    
    public void CurrencyConversion(decimal amount, string toCurrency)
    {
        
        const decimal gelToUsdRate = 0.377m; 
        const decimal gelToEurRate = 0.35m; 

        var balanceUsd = TransactionHistory[0].AmountUsd;
        var balanceGel = TransactionHistory[0].AmountGel;
        var balanceEur = TransactionHistory[0].AmountEur;
        

        decimal convertedAmount = 0;
        
        Transaction? conversionTransaction;
        switch (toCurrency.ToLower())
        {
            case "usd":
                convertedAmount = amount * gelToUsdRate;
                
                conversionTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Currency Conversion to USD",
                    AmountGel = balanceGel-amount,
                    AmountUsd = balanceUsd + convertedAmount,
                    AmountEur = balanceEur
            
                };

                TransactionHistory.Insert(0,conversionTransaction);
                Console.WriteLine($"Added {convertedAmount}USD successfully.");
                break;
            
            case "eur":
                convertedAmount = amount * gelToEurRate;
                conversionTransaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Currency Conversion to EUR",
                    AmountGel = balanceGel-amount,
                    AmountUsd = balanceUsd,
                    AmountEur = balanceEur+convertedAmount
            
                };

                TransactionHistory.Insert(0,conversionTransaction);
                Console.WriteLine($"Added {convertedAmount}EUR successfully.");
                break;
            default:
                Console.WriteLine("Invalid currency for conversion. Can only convert to USD or EUR.");
                break;
        }

      
    }
    
    
    
}