import pandas as pd
import math


def validate_transactions(transactions: list, checks: list = []) -> dict:

    completeness_errors = []
    datatype_errors = []
    binary_errors = []
    format_errors = []
    allowed_value_errors = []
    consistency_errors = []

    int_columns = ["step", "isFraud", "isFlaggedFraud"]
    float_columns = ["amount", "oldbalanceOrig", "newbalanceOrig", "oldbalanceDest", "newbalanceDest"]
    str_columns = ["type", "nameOrig", "nameDest"]

    # If no checks specified, run everything
    run_all = len(checks) == 0

    for index, transaction in enumerate(transactions):

        #Completeness
        if run_all or "completeness" in checks:
            all_columns = int_columns + float_columns + str_columns
            for column in all_columns:
                value = transaction.get(column)
                if value is None or str(value).strip() == "":
                    completeness_errors.append(
                        f"Row:{index} | Column:{column} | Issue: Missing value"
                    )

        #Data type
        if run_all or "datatype" in checks:
            for col in int_columns:
                try:
                    val = transaction.get(col)
                    if val is None or pd.isna(val):
                        raise ValueError()
                    converted = float(val)
                    if math.isinf(converted) or math.isnan(converted):
                        raise ValueError()
                    if converted != int(converted):
                        raise ValueError()
                except (ValueError, TypeError, OverflowError):
                    datatype_errors.append(
                        f"Row:{index} | Column:{col} | Issue: Expected integer got '{val}'"
                    )

            for col in float_columns:
                try:
                    val = transaction.get(col)
                    if val is None or pd.isna(val):
                        raise ValueError()
                    converted = float(val)
                    if math.isinf(converted) or math.isnan(converted):
                        raise ValueError()
                except (ValueError, TypeError, OverflowError):
                    datatype_errors.append(
                        f"Row:{index} | Column:{col} | Issue: Expected decimal got '{val}'"
                    )

        #Binary
        if run_all or "binary" in checks:
            binary_columns = ["isFraud", "isFlaggedFraud"]
            for col in binary_columns:
                val = transaction.get(col)
                if val not in [0, 1, "0", "1", True, False]:
                    binary_errors.append(
                        f"Row:{index} | Column:{col} | Issue: Expected 0 or 1 got '{val}'"
                    )

        #Format
        if run_all or "format" in checks:
            name_orig = str(transaction.get("nameOrig", "")).strip()
            if name_orig and not name_orig.startswith(("C", "M")):
                format_errors.append(
                    f"Row:{index} | Column:nameOrig | Issue: Invalid format got '{name_orig}'"
                )

            name_dest = str(transaction.get("nameDest", "")).strip()
            if name_dest and not name_dest.startswith(("C", "M")):
                format_errors.append(
                    f"Row:{index} | Column:nameDest | Issue: Invalid format got '{name_dest}'"
                )

            amount_str = str(transaction.get("amount", "")).strip()
            if any(char in amount_str for char in ["£", "$", "#", ","]):
                format_errors.append(
                    f"Row:{index} | Column:amount | Issue: Invalid symbol found in '{amount_str}'"
                )

            type_val = str(transaction.get("type", ""))
            if type_val != type_val.strip():
                format_errors.append(
                    f"Row:{index} | Column:type | Issue: Whitespace found in '{type_val}'"
                )
            if type_val.strip() != type_val.strip().upper():
                format_errors.append(
                    f"Row:{index} | Column:type | Issue: Wrong case found in '{type_val}'"
                )

        #Allowed values
        if run_all or "allowedvalues" in checks:
            valid_types = ["PAYMENT", "CASH_OUT", "DEBIT", "CASH_IN", "TRANSFER"]
            type_check = str(transaction.get("type", "")).strip().upper()
            if type_check and type_check not in valid_types:
                allowed_value_errors.append(
                    f"Row:{index} | Column:type | Issue: Invalid transaction type got '{type_check}'"
                )

            amount_val = transaction.get("amount")
            if amount_val is not None:
                try:
                    if float(amount_val) <= 0:
                        allowed_value_errors.append(
                            f"Row:{index} | Column:amount | Issue: Amount must be greater than 0 got '{amount_val}'"
                        )
                except (ValueError, TypeError):
                    pass

        #Consistency
        if run_all or "consistency" in checks:
            try:
                amount = float(transaction.get("amount", 0))
                old_orig = float(transaction.get("oldbalanceOrig", 0))
                new_orig = float(transaction.get("newbalanceOrig", 0))
                old_dest = float(transaction.get("oldbalanceDest", 0))
                new_dest = float(transaction.get("newbalanceDest", 0))

                sender_change = round(old_orig - new_orig, 2)
                expected_amount = round(amount, 2)
                if sender_change != expected_amount and old_orig != 0:
                    consistency_errors.append(
                        f"Row:{index} | Issue: Sender balance change {sender_change} does not match amount {expected_amount}"
                    )

                recipient_change = round(new_dest - old_dest, 2)
                if recipient_change != expected_amount and old_dest != 0:
                    consistency_errors.append(
                        f"Row:{index} | Issue: Recipient balance change {recipient_change} does not match amount {expected_amount}"
                    )

                name_orig_check = str(transaction.get("nameOrig", "")).strip()
                name_dest_check = str(transaction.get("nameDest", "")).strip()
                if name_orig_check and name_dest_check and name_orig_check == name_dest_check:
                    consistency_errors.append(
                        f"Row:{index} | Issue: Sender and recipient are the same account '{name_orig_check}'"
                    )

            except (ValueError, TypeError):
                pass

    total_rows = len(transactions)
    total_errors = (
        len(completeness_errors) +
        len(datatype_errors) +
        len(binary_errors) +
        len(format_errors) +
        len(allowed_value_errors) +
        len(consistency_errors)
    )
    passed_rows = total_rows - total_errors

    return {
        "summary": {
            "total_rows": total_rows,
            "total_errors": total_errors,
            "passed_rows": passed_rows,
            "checks_run": checks if checks else ["all"]
        },
        "completeness_errors": completeness_errors,
        "datatype_errors": datatype_errors,
        "binary_errors": binary_errors,
        "format_errors": format_errors,
        "allowed_value_errors": allowed_value_errors,
        "consistency_errors": consistency_errors
    }