# Data enrichment module
#     this Module performs data enrichment on the transaction dataset PaySim
# It generates additional transaction attributes and produces an enriched transaction dataset
def enrich_transactions(transactions: list, operations: list = []) -> dict:
#Stores enriched rows
    enriched_rows = []
    #Run all data cleaning operations if none is selected
    run_all = len(operations) == 0
    for transaction in transactions:
    # Create a copy to preserve the original transaction
        t = dict(transaction)
    # Generate transaction risk level
        if run_all or "risklevel" in operations:
            try:
                amount = float(t.get("amount", 0))
                if amount < 1000:
                    t["riskLevel"] = "Low"
                elif amount <= 10000:
                    t["riskLevel"] = "Medium"
                else:
                    t["riskLevel"] = "High"
            except (ValueError, TypeError):
                t["riskLevel"] = "Unknown"
    # Calculate sender balance difference
        if run_all or "senderbalancedifference" in operations:
            try:
                old_balance = float(t.get("oldbalanceOrig", 0))
                new_balance = float(t.get("newbalanceOrig", 0))
                t["senderBalanceDifference"] = round(
                    old_balance - new_balance,
                    2
                )
            except (ValueError, TypeError):
                t["senderBalanceDifference"] = None
    # Calculate recipient balance difference
        if run_all or "recipientbalancedifference" in operations:
            try:
                old_balance = float(t.get("oldbalanceDest", 0))
                new_balance = float(t.get("newbalanceDest", 0))
                t["recipientBalanceDifference"] = round(
                    new_balance - old_balance,
                    2
                )
            except (ValueError, TypeError):
                t["recipientBalanceDifference"] = None
        enriched_rows.append(t)
    return {
        "summary": {
            "total_input": len(transactions),
            "total_enriched": len(enriched_rows),
            "operations_run": operations if operations else ["all"]
        },
        "enriched_transactions": enriched_rows
    }