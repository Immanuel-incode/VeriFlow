namespace Transaction
{
    public class Transaction
    {
        public int Step { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public double OldBalanceOrig { get; set; }
        public double NewBalanceOrig { get; set; }
        public string MerchantId { get; set; } = string.Empty;
        public double OldBalanceDest { get; set; }
        public double NewBalanceDest { get; set; }
        public bool IsFraud { get; set; }
        public bool IsFlaggedFraud { get; set; }
    }
}