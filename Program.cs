using System;
using System.IO;
using System.Collections.Generic;
using System.Transactions;

namespace Transaction
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"Database\data\raw\paysim_01.csv";

            List<Transaction> transactions = new List<Transaction>();

            string[] lines = File.ReadAllLines(csvPath);

            Console.WriteLine($"Found {lines.Length - 1} transaction to process");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                Transaction t = new Transaction();
                t.Step = int.Parse(values[0]);
                t.Type = values[1];
                t.Amount = double.Parse(values[2]);
                t.CustomerId = values[3];
                t.OldBalanceOrig = double.Parse(values[4]);
                t.NewBalanceOrig = double.Parse(values[5]);
                t.MerchantId = values[6];
                t.OldBalanceDest = double.Parse(values[7]);
                t.NewBalanceDest = double.Parse(values[8]);
                t.IsFraud = values[9] == "1";
                t.IsFlaggedFraud = values[10] == "1";

                transactions.Add(t);
            }

            Console.WriteLine($"Succesfully parsed {transactions.Count} transactions");
            Console.WriteLine($"First transaction: {transactions[0].CustomerId} - {transactions[0].Type} - {transactions[0].Amount}");

            Console.WriteLine("\nValidating transactions, please wait...");

            TransactionChecker checker = new TransactionChecker();
            int validCount = 0;
            int invalidCount = 0;

            foreach (Transaction t in transactions)
            {
                string reason;
                if (checker.IsValid(t, out reason))
                {
                    validCount++;
                }
                else
                {
                    invalidCount++;
                    Console.WriteLine($"Invalid: {t.Step} | {t.CustomerId} | Type: {t.Type} | Amount: {t.Amount} | {t.MerchantId} | Reason: {reason}");
                }
            }

            Console.WriteLine($"Valid: {validCount} | Invalid: {invalidCount}");
        }
    }
}