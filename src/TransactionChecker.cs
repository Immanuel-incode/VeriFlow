using System;
using System.Collections.Generic;

namespace Transaction
{
    public class TransactionChecker
    {
        private static readonly string[] ValidTypes = 
        {
            "PAYMENT", "TRANSFER", "CASH_OUT", "CASH_IN", "DEBIT"
        };

        public bool IsValid(Transaction t, out string reason)
        {
            if (t.Step <= 0)
            {
                reason = "Step must be greater than zero";
                return false;
            }

            if (t.Amount <= 0)
            {
                reason = "Amount must be greater than zero";
                return false;
            }

            if (!t.CustomerId.StartsWith("C"))
            {
                reason = "CustomerId must start with C";
                return false;
            }

            if (!t.MerchantId.StartsWith("C") && !t.MerchantId.StartsWith("M"))
            {
                reason = "MerchantId must start with C or M";
                return false;
            }

            if (Array.IndexOf(ValidTypes, t.Type) == -1)
            {
                reason = $"Type '{t.Type}' is not a valid transaction type";
                return false;
            }

            reason = "Valid";
            return true;
        }
    }
}