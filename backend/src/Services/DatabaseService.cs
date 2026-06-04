using Npgsql;
using Backend.Models;
using System.Data;

namespace Backend.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertTransactions(List<Transaction> transactions)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            foreach (var transaction in transactions)
            using (var command = new NpgsqlCommand("INSERT INTO transactions (hour, transaction_type, amount, sender_id, sender_balance_before, sender_balance_after, recipient_id, recipient_balance_before, recipient_balance_after, is_fraud, is_flagged_fraud) VALUES (@hour, @transaction_type, @amount, @sender_id, @sender_balance_before, @sender_balance_after, @recipient_id, @recipient_balance_before, @recipient_balance_after, @is_fraud, @is_flagged_fraud)", connection))
                {
                    command.Parameters.AddWithValue("@hour", transaction.Hour);
                    command.Parameters.AddWithValue("@transaction_type", transaction.TransactionType);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@sender_id", transaction.SenderId);
                    command.Parameters.AddWithValue("@sender_balance_before", transaction.SenderBalanceBefore);
                    command.Parameters.AddWithValue("@sender_balance_after", transaction.SenderBalanceAfter);
                    command.Parameters.AddWithValue("@recipient_id", transaction.RecipientId);
                    command.Parameters.AddWithValue("@recipient_balance_before", transaction.RecipientBalanceBefore);
                    command.Parameters.AddWithValue("@recipient_balance_after", transaction.RecipientBalanceAfter);
                    command.Parameters.AddWithValue("@is_fraud", transaction.IsFraud);
                    command.Parameters.AddWithValue("@is_flagged_fraud", transaction.IsFlaggedFraud);

                    command.ExecuteNonQuery();
                }

            
        }
        
    }
}