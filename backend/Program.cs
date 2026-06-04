using Backend.Services;
using Microsoft.Extensions.Configuration;

var service = new TransactionService();

var csvPath = @"../data/paysim_01.csv";

var outputPath = @"../data/sample.csv";

Console.WriteLine("How many rows would you like to extract?");

var input = Console.ReadLine();

var rowCount = int.Parse(input!);

var transactions = service.LoadCsv(csvPath, rowCount);

service.SaveCsv(transactions, outputPath);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection");

var dbService = new DatabaseService(connectionString!);
dbService.InsertTransactions(transactions);
Console.WriteLine($"{rowCount} rows inserted into PostgreSQL.");

Console.WriteLine($"Done! {rowCount} rows have been save to {outputPath}");

Console.WriteLine("\n Here are the transactions");

foreach (var transaction in transactions)
{
    Console.WriteLine($"Hour: {transaction.Hour} | Type: {transaction.TransactionType} | Amount: {transaction.Amount} | Fraud: {transaction.IsFraud}");
}
