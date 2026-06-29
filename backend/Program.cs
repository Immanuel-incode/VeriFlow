using Backend.Services;
using Backend.Models;
using System.Text;
using System.Text.Json;

var service = new TransactionService();
var csvPath = @"../data/paysim_01.csv";

Console.WriteLine("Transaction + Fraud Pipeline\n");
Console.WriteLine("How many rows would you like to load?");
var rowCount = int.Parse(Console.ReadLine()!);
Console.WriteLine("Which row would you like to start from? (Enter 0 for first row)");
var startRow = int.Parse(Console.ReadLine()!) + 1;

var transactions = service.LoadCsv(csvPath, startRow, rowCount);
Console.WriteLine($"\n{transactions.Count} transactions loaded.\n");

//Function selector
Console.WriteLine("Select which operation to run:");
Console.WriteLine("  You can run any combination independently.");
Console.WriteLine("  Type the numbers you want separated by spaces.\n");
Console.WriteLine("  1. Data validation");
Console.WriteLine("  2. Data cleaning");
Console.Write("\nYour selection (e.g. 1  or  1 2): ");

var selectionInput = Console.ReadLine() ?? "";

var selectedOperation = selectionInput
    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
    .ToHashSet();

if (selectedOperation.Count == 0)
{
    Console.WriteLine("\nNo stages selected. Exiting.");
    return;
}

Console.WriteLine($"\nRunning: {string.Join(" + ", selectedOperation.OrderBy(s => s))}\n");

//Run selected stages independently
if (selectedOperation.Contains("1"))
{
    Console.WriteLine("Operation 1: Data validation");
    await RunValidation(transactions);
}

if (selectedOperation.Contains("2"))
{
    Console.WriteLine("--- Stage 2: Data cleaning ---");
    RunCleaning(transactions);
}

//Operation methods
async Task RunValidation(List<Transaction> transactions)
{
    Console.WriteLine("\n  Which checks would you like to run?");
    Console.WriteLine("  Type the numbers separated by spaces, or press Enter to run all.\n");
    Console.WriteLine("  1. Completeness");
    Console.WriteLine("  2. Data type");
    Console.WriteLine("  3. Binary");
    Console.WriteLine("  4. Format");
    Console.WriteLine("  5. Allowed values");
    Console.WriteLine("  6. Consistency");
    Console.Write("\n  Your selection (or Enter for all): ");

    var checkInput = Console.ReadLine() ?? "";

    // Map numbers to check names Python expects
    var checkMap = new Dictionary<string, string>
    {
        { "1", "completeness" },
        { "2", "datatype" },
        { "3", "binary" },
        { "4", "format" },
        { "5", "allowedvalues" },
        { "6", "consistency" }
    };

    // Convert selected numbers to check names
    var selectedChecks = checkInput
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Where(k => checkMap.ContainsKey(k))
        .Select(k => checkMap[k])
        .ToList();

    if (selectedChecks.Count == 0)
        Console.WriteLine("\n  > Running all checks...");
    else
        Console.WriteLine($"\n  > Running: {string.Join(", ", selectedChecks)}");

    Console.WriteLine("  > Sending transactions to Python validation...");

    var transactionDicts = transactions.Select(t => new Dictionary<string, object?>
    {
        { "step",           t.Hour },
        { "type",           t.TransactionType },
        { "amount",         t.Amount },
        { "nameOrig",       t.SenderId },
        { "oldbalanceOrig", t.SenderBalanceBefore },
        { "newbalanceOrig", t.SenderBalanceAfter },
        { "nameDest",       t.RecipientId },
        { "oldbalanceDest", t.RecipientBalanceBefore },
        { "newbalanceDest", t.RecipientBalanceAfter },
        { "isFraud",        t.IsFraud ? 1 : 0 },
        { "isFlaggedFraud", t.IsFlaggedFraud ? 1 : 0 }
    }).ToList();

    // Send both transactions and selected checks to Python
    var payload = new { transactions = transactionDicts, checks = selectedChecks };
    var json = JsonSerializer.Serialize(payload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using var client = new HttpClient();
    var response = await client.PostAsync("http://localhost:8000/validate", content);
    var responseBody = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

    var summary = result.GetProperty("summary");
    Console.WriteLine($"\n  Validation complete:");
    Console.WriteLine($"  Total rows:    {summary.GetProperty("total_rows")}");
    Console.WriteLine($"  Passed:        {summary.GetProperty("passed_rows")}");
    Console.WriteLine($"  Total errors:  {summary.GetProperty("total_errors")}");
    Console.WriteLine($"  Checks run:    {summary.GetProperty("checks_run")}");

    Console.WriteLine("\n  Error breakdown:");
    PrintErrors(result, "completeness_errors",  "Completeness");
    PrintErrors(result, "datatype_errors",       "Data type");
    PrintErrors(result, "binary_errors",         "Binary");
    PrintErrors(result, "format_errors",         "Format");
    PrintErrors(result, "allowed_value_errors",  "Allowed values");
    PrintErrors(result, "consistency_errors",    "Consistency");
}
void PrintErrors(JsonElement result, string key, string label)
{
    var errors = result.GetProperty(key);
    var count = errors.GetArrayLength();
    Console.WriteLine($"  {label}: {count} error(s)");
    if (count > 0)
    {
        foreach (var error in errors.EnumerateArray())
        {
            Console.WriteLine($"    - {error}");
        }
    }
}

void RunCleaning(List<Transaction> transactions)
{
    Console.WriteLine("  > Cleaning — coming soon.");
}
