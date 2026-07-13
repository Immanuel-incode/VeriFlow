from fastapi import FastAPI, Form
from fastapi.middleware.cors import CORSMiddleware
from typing import List
import pandas as pd
from pathlib import Path

from validation import validate_transactions
from cleaning import clean_transactions
from enrichment import enrich_transactions

app = FastAPI()

#Allows requests from the React frontend
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:3000",
        "http://localhost:5173",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

#API health check endpoint
@app.get("/")
def root():
    return {"message": "Transaction + Fraud Detection Pipeline API is running"}
#Path to the paySim dataset
DATASET_PATH = (
    Path(__file__).resolve().parents[2]
    / "data"
    / "paysim_01.csv"
)
#Loads the dataset and convert it into a list of transaction dictionaries
def load_transactions():
#Reads dataset into a dataFrame, Replace missing values with empty strings and then converts that dataframe into a list of dictionaries
    df = pd.read_csv(DATASET_PATH)
    df = df.fillna("")
    return df.to_dict(orient="records")
#validation endpoint
@app.post("/validate")
async def validate(
    checks: List[str] = Form(default=[]),
):
#Loads the transaction dataset
    transaction_list = load_transactions()
#Runs the selected validation checks
    return validate_transactions(
        transaction_list,
        checks
    )
#Data cleaning endpoint
@app.post("/clean")
async def clean(
    checks: List[str] = Form(default=[]),
):
    transaction_list = load_transactions()
#Converts frontend checkbox IDs into backend operation names
    operation_map = {
        "remove_missing": "removemissing",
        "fill_missing": "fillmissing",
        "remove_duplicates": "removeduplicates",
        "normalize": "normalize",
        "stripsymbols": "stripsymbols",
        "trimwhitespace": "trimwhitespace",
        "fixcase": "fixcase",
        "fixspelling": "fixspelling",
    }
#Converts the selected frontend options into backend operations
    operations = [
        operation_map.get(item, item)
        for item in checks
    ]
#Runs the selected cleaning operations
    return clean_transactions(
        transaction_list,
        operations
    )
#Data enrichment endpoint
@app.post("/enrich")
async def enrich(
    checks: List[str] = Form(default=[]),
):
#Load the transaction dataset
    transaction_list = load_transactions()
#Map frontend checkbox IDs to backend enrichment operations
    operation_map = {
            "generate_risk_level": "risklevel",
            "calculate_sender_balance_difference": "senderbalancedifference",
            "calculate_recipient_balance_difference": "recipientbalancedifference",
        }
#Convert selected frontend options into backend operations
    operations = [
        operation_map.get(item, item)
        for item in checks
    ]
#Run the selected enrichment operations
    return enrich_transactions(
        transaction_list,
        operations
    )