def clean_transactions(transactions: list, operations: list = []) -> dict:

    cleaned_rows = []
    quarantined_rows = []
    duplicates_removed = 0
    seen_rows = set()

    run_all = len(operations) == 0

    for index, transaction in enumerate(transactions):
        t = dict(transaction)
        should_quarantine = False

        #Removes symbols 
        if run_all or "stripsymbols" in operations:
            amount_str = str(t.get("amount", "")).strip()
            if any(char in amount_str for char in ["£", "$", "#", ",", "~"]):
                cleaned = amount_str.replace("£", "").replace("$", "").replace("#", "").replace(",", "").replace("~", "")
                try:
                    float(cleaned)
                    t["amount"] = cleaned
                except ValueError:
                    should_quarantine = True
                    quarantined_rows.append({
                        "row": index,
                        "reason": f"amount '{amount_str}' could not be cleaned"
                    })

        #Remove extra or whitespaces
        if run_all or "trimwhitespace" in operations:
            for field in ["type", "nameOrig", "nameDest"]:
                val = str(t.get(field, ""))
                trimmed = val.strip()
                if val != trimmed:
                    t[field] = trimmed

        #Fix word cases
        if run_all or "fixcase" in operations:
            type_val = str(t.get("type", "")).strip()
            if type_val and type_val != type_val.upper():
                t["type"] = type_val.upper()

        #Fix spelling
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

        #Duplicate detection
        if run_all or "removeduplicates" in operations:
            fingerprint = f"{t.get('step')}|{t.get('nameOrig')}|{t.get('nameDest')}|{t.get('amount')}"
            if fingerprint in seen_rows:
                duplicates_removed += 1
                continue
            seen_rows.add(fingerprint)

        #Routing
        if not should_quarantine:
            cleaned_rows.append(t)

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