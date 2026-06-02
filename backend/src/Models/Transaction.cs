namespace Backend.Models
{
    public class Transaction
    {
        public int Step { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public decimal OldBalanceOrigin { get; set; }
        public decimal NewBalanceOrigin { get; set; }
        public string MerchantId { get; set; } = string.Empty;
        public decimal OldBalanceDest {get; set; }
        public decimal NewBalanceDest { get; set; }
        public bool IsFraud { get; set; }
        public bool IsFlaggedFraud { get; set; }
    }
}