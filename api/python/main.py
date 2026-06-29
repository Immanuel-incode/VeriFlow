from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List
from validation import validate_transactions

app = FastAPI()

# Allow React (localhost:3000) to call this API
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],
    allow_methods=["*"],
    allow_headers=["*"],
)


class TransactionRequest(BaseModel):
    transactions: List[dict]
    checks: List[str] = []


@app.get("/")
def root():
    return {"message": "Transaction Fraud Pipeline API is running"}


@app.post("/validate")
def validate(request: TransactionRequest):
    result = validate_transactions(request.transactions, request.checks)
    return result