using Backend.Models;

namespace Backend.Services
{
    public class TransactionService
    {
        public List<Transaction> LoadCsv (string filePath, int rowCount)
        {
            var transactions = new List<Transaction>();
            var lines = File.ReadLines(filePath).Skip(1).Take(rowCount);

            foreach (var line in lines)
            {
                var values = line.Split(',');

                var transaction = new Transaction();

                transaction.Step = int.Parse(values[0]);
                transaction.Type = values[1];
                transaction.Amount = decimal.Parse(values[2]);
                transaction.CustomerId = values[3];
                transaction.OldBalanceOrigin = decimal.Parse(values[4]);
                transaction.NewBalanceOrigin = decimal.Parse(values[5]);
                transaction.MerchantId = values[6];
                transaction.OldBalanceDest = decimal.Parse(values[7]);
                transaction.NewBalanceDest = decimal.Parse(values[8]);
                transaction.IsFraud = values[9] == "1";
                transaction.IsFlaggedFraud = values[10] == "1";

                transactions.Add(transaction);
            }

            return transactions;
        }

        public void SaveCsv(List<Transaction> transactions, string outputPath)
        {
            var lines = new List<string>();
            lines.Add("step,type,amount,customerId,oldBalanceOrigin,newBalanceOrigin,merchantId,oldBalanceDest,newBalanceDest,isFraud,isFlaggedFraud");

            foreach (var transaction in transactions)
            {
                lines.Add($"{transaction.Step},{transaction.Type},{transaction.Amount},{transaction.CustomerId},{transaction.OldBalanceOrigin},{transaction.NewBalanceOrigin},{transaction.MerchantId},{transaction.OldBalanceDest},{transaction.NewBalanceDest},{transaction.IsFraud},{transaction.IsFlaggedFraud}");
            }

            File.WriteAllLines(outputPath, lines);
        }
    }
}