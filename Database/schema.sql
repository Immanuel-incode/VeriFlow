-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Dimension Tables
CREATE TABLE dim_customers (
    customer_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    country VARCHAR(60) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE dim_merchants (
    merchant_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    merchant_name VARCHAR(150) NOT NULL,
    category VARCHAR(80) NOT NULL,
    country VARCHAR(60) NOT NULL
);

CREATE TABLE dim_date (
    date_id SERIAL PRIMARY KEY,
    full_date DATE NOT NULL UNIQUE,
    day_of_week VARCHAR(10) NOT NULL,
    month INTEGER NOT NULL,
    year INTEGER NOT NULL,
    is_weekend BOOLEAN NOT NULL
);

-- Fact Table
CREATE TABLE fact_transactions (
    transaction_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    customer_id UUID NOT NULL REFERENCES dim_customers(customer_id),
    merchant_id UUID NOT NULL REFERENCES dim_merchants(merchant_id),
    date_id INTEGER NOT NULL REFERENCES dim_date(date_id),
    amount NUMERIC(12, 2) NOT NULL,
    currency VARCHAR(10) NOT NULL DEFAULT 'GBP',
    status VARCHAR(20) NOT NULL,
    channel VARCHAR(20) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Output Tables
CREATE TABLE fraud_flags (
    flag_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_id UUID NOT NULL REFERENCES fact_transactions(transaction_id),
    rule_triggered VARCHAR(100) NOT NULL,
    confidence NUMERIC(5, 2),
    status VARCHAR(20) NOT NULL DEFAULT 'open',
    flagged_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE risk_scores (
    score_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_id UUID NOT NULL UNIQUE REFERENCES fact_transactions(transaction_id),
    score NUMERIC(5, 2) NOT NULL,
    risk_level VARCHAR(20) NOT NULL,
    scored_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Staging Table
CREATE TABLE raw_transactions (
    id SERIAL PRIMARY KEY,
    raw_data JSONB,
    source_file VARCHAR(255),
    loaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    processed BOOLEAN DEFAULT FALSE
);

-- Indexes
CREATE INDEX idx_transactions_customer ON fact_transactions(customer_id);
CREATE INDEX idx_transactions_merchant ON fact_transactions(merchant_id);
CREATE INDEX idx_transactions_date ON fact_transactions(date_id);
CREATE INDEX idx_fraud_flags_transaction ON fraud_flags(transaction_id);
CREATE INDEX idx_risk_scores_transaction ON risk_scores(transaction_id);
CREATE INDEX idx_raw_transactions_processed ON raw_transactions(processed);