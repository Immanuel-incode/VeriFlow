CREATE TABLE IF NOT EXISTS transactions (
    id SERIAL PRIMARY KEY,
    hour INTEGER NOT NULL,
    transaction_type VARCHAR(20) NOT NULL,
    amount NUMERIC(12, 2) NOT NULL,
    sender_id VARCHAR(20) NOT NULL,
    sender_balance_before NUMERIC(12, 2) NOT NULL,
    sender_balance_after NUMERIC(12, 2) NOT NULL,
    recipient_id VARCHAR(20) NOT NULL,
    recipient_balance_before NUMERIC(12, 2) NOT NULL,
    recipient_balance_after NUMERIC(12, 2) NOT NULL,
    is_fraud BOOLEAN NOT NULL,
    is_flagged_fraud BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_transactions_fraud ON transactions(is_fraud);
CREATE INDEX idx_transactions_type ON transactions(transaction_type);
CREATE INDEX idx_transactions_sender ON transactions(sender_id);
CREATE INDEX idx_transactions_amount ON transactions(amount);
