import React, { useState } from "react";

interface Props {
  onResults: (data: any) => void;
}
//Available checks for data validation
const VALIDATION_CHECKS = [
  {id: "completeness", label: "Completeness Check",},
  {id: "datatype", label: "Data Type Check",},
  {id: "binary", label: "Binary Value Check",},
  {id: "format", label: "Format Check",},
  {id: "allowedvalues", label: "Allowed Values Check",},
  {id: "consistency", label: "Consistency Check",},
];
//Available operations for data cleaning
const CLEANING_OPERATIONS = [
  {id: "stripsymbols", label: "Strip Currency Symbols",},
  {id: "trimwhitespace", label: "Trim Whitespace",},
  {id: "fixcase", label: "Fix Text Case",},
  {id: "fixspelling", label: "Fix Spelling",},
  {id: "removeduplicates", label: "Remove Duplicate Records",},
];
//Available enrichment operations
const ENRICHMENT_OPERATIONS = [
  {id: "generate_risk_level", label: "Generate Risk Level",},
  {id: "calculate_sender_balance_difference", label: "Calculate Sender Balance Difference",},
  {id: "calculate_recipient_balance_difference", label: "Calculate Recipient Balance Difference",},
];
const PipelineForm: React.FC<Props> = ({ onResults }) => {
//stores the selected pipeline operations
  const [operation, setOperation] = useState<
    "validation" | "cleaning" | "enrichment"
  >("validation");
//Stores the selected options for the current pipeline stage
  const [selectedChecks, setSelectedChecks] = useState<string[]>([]);
//Tracks if the pipeline is still running
  const [loading, setLoading] = useState(false);
//Add or removes a selected checkbox operation
  const handleCheckbox = (id: string) => {
    setSelectedChecks((prev) =>
      prev.includes(id)
        ? prev.filter((item) => item !== id)
        : [...prev, id]
    );
  };
//Submts the selected operation to the backend
  const handleSubmit = async (
    e: React.FormEvent<HTMLFormElement>
  ) => {
//prevents the page from refreshing
    e.preventDefault();
//Ensures at least one option is selected
    if (selectedChecks.length === 0) {
      alert("Please select at least one option.");
      return;
    }
//Prepares a form data for the API request
    const formData = new FormData();
//Adds selected options to the request
    selectedChecks.forEach((item) =>
      formData.append("checks", item)
    );

    setLoading(true);

    try {
//Determines which endpoint to call upon
      const endpoint =
        operation === "validation"
          ? "http://127.0.0.1:8000/validate"
          : operation === "cleaning"
          ? "http://127.0.0.1:8000/clean"
          : "http://127.0.0.1:8000/enrich";
//Sends the selected option to the backend
      const response = await fetch(endpoint, {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error("Pipeline request failed.");
      }

      const data = await response.json();
//Identifies which operation generated the results
      data.operation = operation;
//Passes the result to the parent component
      onResults(data);

    } catch (err) {
      console.error(err);
      alert("An error occurred while processing the dataset.");
    } finally {
//Re-enables the interface after processing
      setLoading(false);
    }
  };

  return (
    <form className="pipeline-form" onSubmit={handleSubmit}>
      <h2>Transaction Processing Pipeline</h2>
      <p style={{ marginBottom: "20px", color: "#666" }}>The PaySim transaction dataset has been loaded automatically by the system. Select an operation and the checks to perform.</p>
      <div style={{ marginBottom: "20px" }}>
        <h3>Select Operation</h3>

        <label style={{ marginRight: "20px" }}>
          <input
            type="radio"
            value="validation"
            checked={operation === "validation"}
            onChange={() => {
//switch to validation mode
              setOperation("validation");
//Clears previous selected option
              setSelectedChecks([]);
            }}
          />
          Data Validation
        </label>

        <label>
          <input
            type="radio"
            value="cleaning"
            checked={operation === "cleaning"}
            onChange={() => {
//Switch to data cleaning mode
              setOperation("cleaning");
//Clears previous selected option
              setSelectedChecks([]);
            }}
          />
          Data Cleaning
        </label>
        <label style={{ marginLeft: "20px" }}>
        <input
          type="radio"
          value="enrichment"
          checked={operation === "enrichment"}
          onChange={() => {
//Switch to data enrichment mode
            setOperation("enrichment");
//Clears previous selected option
            setSelectedChecks([]);
          }}
        />
        Data Enrichment
      </label>
      </div>

      <div style={{ marginBottom: "20px" }}>
        <h3>
          {operation === "validation"
            ? "Validation Checks"
            : operation === "cleaning"
            ? "Cleaning Operations"
            : "Enrichment Operations"}
        </h3>
{/*Displays the selected pipeline operations*/}
        {(operation === "validation"
            ? VALIDATION_CHECKS
            : operation === "cleaning"
            ? CLEANING_OPERATIONS
            : ENRICHMENT_OPERATIONS
        ).map((item) => (
          <div key={item.id}>
            <label>
              <input
                type="checkbox"
                checked={selectedChecks.includes(item.id)}
                onChange={() => handleCheckbox(item.id)}
              />
              {item.label}
            </label>
          </div>
        ))}
      </div>

      <button type="submit" disabled={loading}>
        {loading
          ? "Processing..."
          : operation === "validation"
          ? "Run Validation"
          : operation === "cleaning"
          ? "Run Cleaning"
          : "Run Enrichment"}
      </button>
    </form>
  );
};

export default PipelineForm;