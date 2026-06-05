using Npgsql;
using Api.Models;

namespace Api.Repositories
{
    public class TransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Transaction> GetAll()
        {
            var transactions = new List<Transaction>();

            using var connection = new NpgsqlConnection(_connectionString);

            connection.Open();

            using var command = new NpgsqlCommand("SELECT id, hour, transaction_type, amount, sender_id, sender_balance_before, sender_balance_after, recipient_id, recipient_balance_before, recipient_balance_after, is_fraud, is_flagged_fraud, created_at FROM transactions", connection);

            using var reader = command.ExecuteReader(); 

            while (reader.Read())
            {
                var transaction = new Transaction
                {
                    Id = reader.GetInt32(0),
                    Hour = reader.GetInt32(1),
                    TransactionType = reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    SenderId = reader.GetString(4),
                    SenderBalanceBefore = reader.GetDecimal(5),
                    SenderBalanceAfter = reader.GetDecimal(6),
                    RecipientId = reader.GetString(7),
                    RecipientBalanceBefore = reader.GetDecimal(8),
                    RecipientBalanceAfter = reader.GetDecimal(9),
                    IsFraud = reader.GetBoolean(10),
                    IsFlaggedFraud = reader.GetBoolean(11),
                    CreatedAt = reader.GetDateTime(12)
                };

                transactions.Add(transaction);
            }

            return transactions;
        }

        public Transaction? GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT id, hour, transaction_type, amount, sender_id, sender_balance_before, sender_balance_after, recipient_id, recipient_balance_before, recipient_balance_after, is_fraud, is_flagged_fraud, created_at FROM transactions WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Transaction
                {
                    Id = reader.GetInt32(0),
                    Hour = reader.GetInt32(1),
                    TransactionType = reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    SenderId = reader.GetString(4),
                    SenderBalanceBefore = reader.GetDecimal(5),
                    SenderBalanceAfter = reader.GetDecimal(6),
                    RecipientId = reader.GetString(7),
                    RecipientBalanceBefore = reader.GetDecimal(8),
                    RecipientBalanceAfter = reader.GetDecimal(9),
                    IsFraud = reader.GetBoolean(10),
                    IsFlaggedFraud = reader.GetBoolean(11),
                    CreatedAt = reader.GetDateTime(12)
                };
            }

            return null;
        }

        public List<Transaction> GetFraudOnly()
        {
            var transactions = new List<Transaction>();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT id, hour, transaction_type, amount, sender_id, sender_balance_before, sender_balance_after, recipient_id, recipient_balance_before, recipient_balance_after, is_fraud, is_flagged_fraud, created_at FROM transactions WHERE is_fraud = true", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var transaction = new Transaction
                {
                    Id = reader.GetInt32(0),
                    Hour = reader.GetInt32(1),
                    TransactionType = reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    SenderId = reader.GetString(4),
                    SenderBalanceBefore = reader.GetDecimal(5),
                    SenderBalanceAfter = reader.GetDecimal(6),
                    RecipientId = reader.GetString(7),
                    RecipientBalanceBefore = reader.GetDecimal(8),
                    RecipientBalanceAfter = reader.GetDecimal(9),
                    IsFraud = reader.GetBoolean(10),
                    IsFlaggedFraud = reader.GetBoolean(11),
                    CreatedAt = reader.GetDateTime(12)
                };
                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}