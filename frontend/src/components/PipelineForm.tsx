import React, { useState } from "react";

interface Props {
  onResults: (data: any) => void;
}

const VALIDATION_CHECKS = [
  {id: "completeness", label: "Completeness Check",},
  {id: "datatype", label: "Data Type Check",},
  {id: "binary", label: "Binary Value Check",},
  {id: "format", label: "Format Check",},
  {id: "allowedvalues", label: "Allowed Values Check",},
  {id: "consistency", label: "Consistency Check",},
];

const CLEANING_OPERATIONS = [
  {id: "stripsymbols", label: "Strip Currency Symbols",},
  {id: "trimwhitespace", label: "Trim Whitespace",},
  {id: "fixcase", label: "Fix Text Case",},
  {id: "fixspelling", label: "Fix Spelling",},
  {id: "removeduplicates", label: "Remove Duplicate Records",},
];

const PipelineForm: React.FC<Props> = ({ onResults }) => {
  const [file, setFile] = useState<File | null>(null);

  const [operation, setOperation] = useState<
    "validation" | "cleaning"
  >("validation");

  const [selectedChecks, setSelectedChecks] = useState<string[]>([]);

  const [loading, setLoading] = useState(false);

  const handleCheckbox = (id: string) => {
    setSelectedChecks((prev) =>
      prev.includes(id)
        ? prev.filter((item) => item !== id)
        : [...prev, id]
    );
  };

  const handleSubmit = async (
    e: React.FormEvent<HTMLFormElement>
  ) => {
    e.preventDefault();

    if (!file) {
      alert("Please upload a CSV file.");
      return;
    }

    if (selectedChecks.length === 0) {
      alert("Please select at least one option.");
      return;
    }

    const formData = new FormData();
    formData.append("transactions", file);

    selectedChecks.forEach((item) =>
      formData.append("checks", item)
    );

    setLoading(true);

    try {
      const endpoint =
        operation === "validation"
          ? "http://127.0.0.1:8000/validate"
          : "http://127.0.0.1:8000/clean";

      const response = await fetch(endpoint, {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error("Pipeline request failed.");
      }

      const data = await response.json();

      // Identify which operation produced the results
      data.operation = operation;

      onResults(data);
    } catch (err) {
      console.error(err);
      alert("An error occurred while processing the file.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="pipeline-form" onSubmit={handleSubmit}>
      <h2>Transaction Processing Pipeline</h2>

      <div style={{ marginBottom: "20px" }}>
        <label>
          Upload CSV
          <br />
          <input
            type="file"
            accept=".csv"
            onChange={(e) =>
              setFile(e.target.files?.[0] || null)
            }
          />
        </label>
      </div>

      <div style={{ marginBottom: "20px" }}>
        <h3>Select Operation</h3>

        <label style={{ marginRight: "20px" }}>
          <input
            type="radio"
            value="validation"
            checked={operation === "validation"}
            onChange={() => {
              setOperation("validation");
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
              setOperation("cleaning");
              setSelectedChecks([]);
            }}
          />
          Data Cleaning
        </label>
      </div>

      <div style={{ marginBottom: "20px" }}>
        <h3>
          {operation === "validation"
            ? "Validation Checks"
            : "Cleaning Operations"}
        </h3>

        {(operation === "validation"
          ? VALIDATION_CHECKS
          : CLEANING_OPERATIONS
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
          : "Run Cleaning"}
      </button>
    </form>
  );
};

export default PipelineForm;