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

                transaction.Hour = int.Parse(values[0]);
                transaction.TransactionType = values[1];
                transaction.Amount = decimal.Parse(values[2]);
                transaction.SenderId = values[3];
                transaction.SenderBalanceBefore = decimal.Parse(values[4]);
                transaction.SenderBalanceAfter = decimal.Parse(values[5]);
                transaction.RecipientId = values[6];
                transaction.RecipientBalanceBefore = decimal.Parse(values[7]);
                transaction.RecipientBalanceAfter = decimal.Parse(values[8]);
                transaction.IsFraud = values[9] == "1";
                transaction.IsFlaggedFraud = values[10] == "1";

                transactions.Add(transaction);
            }

            return transactions;
        }

        public void SaveCsv(List<Transaction> transactions, string outputPath)
        {
            var lines = new List<string>();
            lines.Add("hour,transaction_type,amount,sender_id,sender_balance_before,sender_balance_after,recipient_id,recipient_balance_before,recipient_balance_after,is_fraud,is_flagged_fraud");

            foreach (var transaction in transactions)
            {
                lines.Add($"{transaction.Hour},{transaction.TransactionType},{transaction.Amount},{transaction.SenderId},{transaction.SenderBalanceBefore},{transaction.SenderBalanceAfter},{transaction.RecipientId},{transaction.RecipientBalanceBefore},{transaction.RecipientBalanceAfter},{transaction.IsFraud},{transaction.IsFlaggedFraud}");
            }

            File.WriteAllLines(outputPath, lines);
        }
    }
}