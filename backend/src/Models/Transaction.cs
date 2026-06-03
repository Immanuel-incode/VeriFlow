namespace Backend.Models
{
    public class Transaction
    {
        public int Hour { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public decimal SenderBalanceBefore { get; set; }
        public decimal SenderBalanceAfter { get; set; }
        public string RecipientId { get; set; } = string.Empty;
        public decimal RecipientBalanceBefore {get; set; }
        public decimal RecipientBalanceAfter { get; set; }
        public bool IsFraud { get; set; }
        public bool IsFlaggedFraud { get; set; }
    }
}