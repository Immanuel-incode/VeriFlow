from fastapi import FastAPI, Form
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from typing import List, Optional
import pandas as pd
from pathlib import Path

from validation import validate_transactions
from cleaning import clean_transactions
from enrichment import enrich_transactions

app = FastAPI()

#Pipeline request model
class PipelineRequest(BaseModel):
    stages: List[str]
    validation_checks: Optional[List[str]] = []
    cleaning_operations: Optional[List[str]] = []
    enrichment_operations: Optional[List[str]] = []
#Converts frontend checkbox IDs into backend operation names
cleaning_operation_map = {
    "remove_missing": "removemissing",
    "fill_missing": "fillmissing",
    "remove_duplicates": "removeduplicates",
    "normalize": "normalize",
    "stripsymbols": "stripsymbols",
    "trimwhitespace": "trimwhitespace",
    "fixcase": "fixcase",
    "fixspelling": "fixspelling",
}
#Map frontend checkbox IDs to backend enrichment operations
enrichment_operation_map = {
        "generate_risk_level": "risklevel",
        "calculate_sender_balance_difference": "senderbalancedifference",
        "calculate_recipient_balance_difference": "recipientbalancedifference",
    }
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
#Converts the selected frontend options into backend operations
    cleaning_operations = [
        cleaning_operation_map.get(item, item)
        for item in checks
    ]
#Runs the selected cleaning operations
    return clean_transactions(
        transaction_list,
        cleaning_operations
    )
#Data enrichment endpoint
@app.post("/enrich")
async def enrich(
    checks: List[str] = Form(default=[]),
):
#Load the transaction dataset
    transaction_list = load_transactions()
#Convert selected frontend options into backend operations
    enrichment_operations = [
        enrichment_operation_map.get(item, item)
        for item in checks
    ]
#Run the selected enrichment operations
    return enrich_transactions(
        transaction_list,
        enrichment_operations
    )

# Multi-stage transaction pipeline endpoint
@app.post("/pipeline")
async def run_pipeline(request: PipelineRequest):
# Load the original transaction dataset
    transactions = load_transactions()
#Stores the results produced by each pipeline stage
    results = {}
#Runs the validation stage if selected
    if "validation" in request.stages:
        validation_result = validate_transactions(
            transactions,
            request.validation_checks
        )
        results["validation"] = validation_result
#Converts the selected frontend options into backend operations
    operations = [
        cleaning_operation_map.get(item, item)
        for item in request.cleaning_operations
    ]
#Runs the cleaning stage if selected
    if "cleaning" in request.stages:
        cleaning_result = clean_transactions(
            transactions, operations
        )
        results["claening"] = cleaning_result
#Uses the cleaned dataset for the next pipeline stage
        transactions = cleaning_result["cleaned_transactions"]
#Convert selected frontend options into backend operations
    operations = [
        enrichment_operation_map.get(item, item)
        for item in request.enrichment_operations
    ]
#Runs the enrichment stage if selected
    if "enrichment" in request.stages:
        enrichment_result = enrich_transactions(
            transactions, operations
        )
        results["enrichment"] = enrichment_result
#Use the enriched dataset for the final output or result
        transactions = enrichment_result["enriched_transactions"]

    results["final_dataset"] = transactions

    return results
