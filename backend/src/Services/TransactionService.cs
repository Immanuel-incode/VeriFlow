using Backend.Models;

namespace Backend.Services
{
    public class TransactionService
    {
        public List<Transaction> LoadCsv (string filePath, int startRow, int rowCount)
        {
            var transactions = new List<Transaction>();
            var lines = File.ReadLines(filePath).Skip(startRow).Take(rowCount);

            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (values.Length != 11)
                {
                    continue;
                }

                var transaction = new Transaction();

                transaction.Step = int.Parse(values[0]);
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
    }
}