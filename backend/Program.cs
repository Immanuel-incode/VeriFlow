using Backend.Services;

var service = new TransactionService();

var csvPath = @"../data/paysim_01.csv";

var outputPath = @"../data/sample.csv";

Console.WriteLine("How many rows would you like to extract?");

var input = Console.ReadLine();

var rowCount = int.Parse(input!);

var transactions = service.LoadCsv(csvPath, rowCount);

service.SaveCsv(transactions, outputPath);

Console.WriteLine($"Done! {rowCount} rows have been save to {outputPath}");

Console.WriteLine("\n Here are the transactions");

foreach (var transaction in transactions)
{
    Console.WriteLine($"Hour: {transaction.Hour} | Type: {transaction.TransactionType} | Amount: {transaction.Amount} | Fraud: {transaction.IsFraud}");
}
