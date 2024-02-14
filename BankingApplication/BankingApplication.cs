using Newtonsoft.Json;
using NLog;

namespace Banking_Application;

internal abstract class BankingApplication
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    public static void Main(string[] args)
    {
        
        var directory = Environment.CurrentDirectory;
        for (var i = 0; i < 3; i++)
            directory = Directory.GetParent(directory).ToString();
        var filePath = Path.Combine(directory, "data.json");
        
        
        try
        {
            InitializeData(filePath);
            var userData = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(filePath));

            while (true)
            {
                Console.WriteLine("Welcome to our ATM!");
                Console.WriteLine("Please enter card details or press 'q' to quit the ATM.");

                Console.Write("Card Number: ");
                var cardNumber = Console.ReadLine();
                if (cardNumber != null && cardNumber.Equals("q", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Exiting the ATM...");
                    break;
                }
                
                Console.Write("Expiration Date (MM/YY): ");
                var expirationDate = Console.ReadLine();

                Console.Write("CVC: ");
                var cvc = Console.ReadLine();

                var currentUser = AuthenticateUser(userData, cardNumber, expirationDate, cvc);

                if (currentUser == null)
                {
                    Logger.Info("User Authentication Failed | Invalid Card Information");
                    Console.WriteLine("Invalid card details. Please try again.");
                    continue;
                }

                var attempts = 0;
                var isAuthenticated = false;
                
                while (attempts < 3)
                {
                    Console.Write("Enter PIN: ");
                    var enteredPin = Console.ReadLine();
                    if (enteredPin == currentUser.CardDetails.PinCode)
                    {
                        isAuthenticated = true;
                        break;
                    }
                    else
                    {
                        attempts++;
                        Console.WriteLine($"Incorrect PIN. {3 - attempts} attempts left.");
                    }
                }

                if (!isAuthenticated)
                {
                    Logger.Info("User Authentication Failed | Incorrect PIN");
                    Console.WriteLine("Too many incorrect attempts. Please try again later.");
                    continue;
                }
                
                while (true)
                {
                    Console.WriteLine($"\nHello {currentUser.FirstName} {currentUser.LastName}!");
                    Console.WriteLine("Select an operation:");
                    Console.WriteLine("1. Balance Enquiry");
                    Console.WriteLine("2. Withdrawal");
                    Console.WriteLine("3. Get 5 Latest Transactions");
                    Console.WriteLine("4. Deposit");
                    Console.WriteLine("5. Change PIN");
                    Console.WriteLine("6. Currency Conversion");
                    Console.WriteLine("7. Exit");

                    var choice = Console.ReadLine(); 
                    
                    var balance = currentUser.TransactionHistory[0].AmountGEL;
                    
                    switch (choice)
                    {
                        case "1":
                            currentUser.CheckBalance();
                            break;
                        
                        case "2":
                            Console.Write("Enter the amount to withdraw: ");
                            if (!decimal.TryParse(Console.ReadLine(), out var amountToWithdraw) || amountToWithdraw <= 0 || amountToWithdraw > balance)
                            {
                                Logger.Info("Money Withdrawal Failed | Invalid Amount");
                                Console.WriteLine("Invalid amount. Please enter a valid number.");
                                continue;
                            }
                            currentUser.MakeWithdrawal(amountToWithdraw);
                            SaveUserData(userData, filePath);
                            break;
                        
                        case "3":
                            currentUser.GetLastTransactions();
                            break;
                        
                        case "4":
                            Console.Write("Enter the amount to deposit: ");
                            if (!decimal.TryParse(Console.ReadLine(), out var amountToDeposit) ||
                                amountToDeposit <= 0)
                            {
                                Logger.Info("Money Deposit Failed | Invalid Amount");
                                Console.WriteLine("Invalid amount. Please enter a valid number.");
                                continue;
                            }
                            currentUser.MakeDeposit(amountToDeposit);
                            SaveUserData(userData, filePath);
                            break;
                        
                        case "5":
                            Console.Write("Enter new PIN: ");
                            var newPin = Console.ReadLine();
                            
                            if (newPin is null || newPin.Length != 4 || !newPin.All(char.IsDigit))
                            {
                                Logger.Info("PIN Change Failed | Invalid PIN");
                                Console.WriteLine("Invalid PIN. PIN must be exactly 4 characters long.");
                                continue;
                            }
                            currentUser.ChangePin(newPin);
                            SaveUserData(userData, filePath);
                            break;
                        
                        case "6":
                            Console.Write("Enter the currency to convert to (USD or EUR): ");
                            var convertToCurrency = Console.ReadLine()?.Trim().ToUpper();
                            if (convertToCurrency != "USD" && convertToCurrency != "EUR")
                            {
                                Logger.Info("Conversion Failed | Invalid Currency");
                                Console.WriteLine("Invalid currency for conversion. Can only convert to USD or EUR.");
                                break;
                            }

                            Console.Write("Enter the amount in GEL to convert: ");
                            if (!decimal.TryParse(Console.ReadLine(), out var amountToConvert) || amountToConvert <= 0 || amountToConvert > balance)
                            {
                                Logger.Info("Conversion Failed | Invalid Amount");
                                Console.WriteLine("Invalid amount. Please enter a valid number.");
                                break;
                            }

                            currentUser.CurrencyConversion(amountToConvert, convertToCurrency);
                            SaveUserData(userData, filePath);
                            break;
                        
                        case "7":
                            Console.WriteLine("Logging out...");
                            break;
                        
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    if (choice == "7")
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred: {ErrorMessage}", ex.Message);
        }
        
    }


    private static void SaveUserData(List<User> userData , string filePath)
    {
        var json = JsonConvert.SerializeObject(userData, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    private static User AuthenticateUser(List<User> users, string cardNumber, string expirationDate, string cvc)
    {
        return users.FirstOrDefault(user => user.CardDetails.CardNumber == cardNumber && user.CardDetails.ExpirationDate == expirationDate && user.CardDetails.CVC == cvc);
    }

    private static void InitializeData(string filePath)
        {
            var users = new List<User>
            {
                new()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    CardDetails = new Card
                    {
                        CardNumber = "1234-5678-9812-3456",
                        ExpirationDate = "12/25",
                        CVC = "123",
                        PinCode = "1234"
                    },
                    TransactionHistory = new List<Transaction>
                    {
                        new()
                        {
                            TransactionDate = DateTime.Parse("2024-01-03T10:15:30Z"),
                            TransactionType = "BalanceInquiry",
                            AmountGEL = 0,
                            AmountUSD = 0,
                            AmountEUR = 0
                        }
                    }
                },
                new()
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    CardDetails = new Card
                    {
                        CardNumber = "9876-5432-1234-5678",
                        ExpirationDate = "11/23",
                        CVC = "456",
                        PinCode = "5678"
                    },
                    TransactionHistory = new List<Transaction>
                    {
                        new()
                        {
                            TransactionDate = DateTime.Parse("2024-01-03T10:15:30Z"),
                            TransactionType = "BalanceInquiry",
                            AmountGEL = 0,
                            AmountUSD = 0,
                            AmountEUR = 0
                        }
                    }
                }
            };
            
            var json = JsonConvert.SerializeObject(users.ToArray(), Formatting.Indented);
            
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            
            File.WriteAllText(filePath, json);
            
        }


}


