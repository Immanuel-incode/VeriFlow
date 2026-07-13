# Data cleaning module
# this Module performs a data quality cleaning on the  transaction dataset PaySim
# It runs various data cleaning activities and produces a data quality report
def clean_transactions(transactions: list, operations: list = []) -> dict:
#Stores transaction that has been cleaned
    cleaned_rows = []
#Stores transations that cannot be cleaned
    quarantined_rows = []
#Tracks number of duplicate transactions removed
    duplicates_removed = 0
#Stores transaction fingerprints for duplicate detection
    seen_rows = set()
#Run all data cleaning operations if none is selected
    run_all = len(operations) == 0
    
#Processes one transaction at a time
    for index, transaction in enumerate(transactions):
        t = dict(transaction)
        should_quarantine = False

#Removes symbols that are not needed
        if run_all or "stripsymbols" in operations:
            amount_str = str(t.get("amount", "")).strip()
            if any(char in amount_str for char in ["£", "$", "#", ",", "~"]):
#Remove invalide characters
                symbols = ["£", "$", "#", ",", "~"]
                cleaned = amount_str
                for symbol in symbols:
                    cleaned = cleaned.replace(symbol, "")
                try:
#Verify if the cleaned values are still numeric
                    float(cleaned)
                    t["amount"] = cleaned
#If cleaning fails move transaction to quarantine
                except ValueError:
                    should_quarantine = True
                    quarantined_rows.append({
                        "row": index,
                        "reason": f"amount '{amount_str}' could not be cleaned"
                    })
#Removes spaces
        if run_all or "trimwhitespace" in operations:
            for field in ["type", "nameOrig", "nameDest"]:
                val = str(t.get(field, ""))
                trimmed = val.strip()
                if val != trimmed:
                    t[field] = trimmed
#Converts transaction type to uppercase
        if run_all or "fixcase" in operations:
            type_val = str(t.get("type", "")).strip()
            if type_val and type_val != type_val.upper():
                t["type"] = type_val.upper()
#Correct spelling mistakes in transaction type column
        if run_all or "fixspelling" in operations:
            spell_fixes = {
                "TRANFER": "TRANSFER",
                "CASHOUT": "CASH_OUT",
                "CASHIN": "CASH_IN",
                "PAYEMENT": "PAYMENT",
                "DEBET": "DEBIT"
            }
            type_val = str(t.get("type", "")).strip().upper()
            if type_val in spell_fixes:
                t["type"] = spell_fixes[type_val]
#Identify duplicate transactions
        if run_all or "removeduplicates" in operations:
            fingerprint = f"{t.get('step')}|{t.get('nameOrig')}|{t.get('nameDest')}|{t.get('amount')}"
#Skip duplicate transactions
            if fingerprint in seen_rows:
                duplicates_removed += 1
                continue
            seen_rows.add(fingerprint)
#Store Cleaned transactions
        if not should_quarantine:
            cleaned_rows.append(t)
#Return data cleaning summary report
    return {
        "summary": {
            "total_input": len(transactions),
            "total_cleaned": len(cleaned_rows),
            "total_quarantined": len(quarantined_rows),
            "duplicates_removed": duplicates_removed
        },
        "cleaned_transactions": cleaned_rows,
        "quarantined_rows": quarantined_rows
    }