import React, { useState } from "react";

interface Props {
  onResults: (data: any) => void;
}

const CHECKS = [
  { id: "completeness",  label: "Completeness" },
  { id: "datatype",      label: "Data type" },
  { id: "binary",        label: "Binary" },
  { id: "format",        label: "Format" },
  { id: "allowedvalues", label: "Allowed values" },
  { id: "consistency",   label: "Consistency" },
];


export default function PipelineForm({ onResults }: Props) {
  const [file, setFile]               = useState<File | null>(null);
  const [selectedChecks, setSelected] = useState<string[]>([]);
  const [rowLimit, setRowLimit]       = useState<string>("");
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");

  function handleFile(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0];
    if (f) setFile(f);
  }

  function toggleCheck(id: string) {
    setSelected(prev =>
      prev.includes(id) ? prev.filter(c => c !== id) : [...prev, id]
    );
  }

  async function handleSubmit() {
    if (!file) { setError("Please upload a CSV file first."); return; }
    setError("");
    setLoading(true);

    try {
      const text = await file.text();
      const lines = text.trim().split("\n");
      const headers = lines[0].split(",");

      const allTransactions = lines.slice(1).map(line => {
        const values = line.split(",");
        const obj: any = {};
        headers.forEach((h, i) => {
          obj[h.trim()] = values[i]?.trim();
        });
        return obj;
      });

      const transactions = rowLimit
        ? allTransactions.slice(0, parseInt(rowLimit))
        : allTransactions;

      const response = await fetch("http://localhost:8000/validate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ transactions, checks: selectedChecks })
      });

      const data = await response.json();
      onResults(data);

    } catch (err) {
      setError("Could not connect to the pipeline. Make sure FastAPI is running.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="form-card">
      <h2>Pipeline configuration</h2>

      <div className="section">
        <label className="section-label">Upload dataset</label>
        <input type="file" accept=".csv" onChange={handleFile} />
        {file && <p className="file-name">Loaded: {file.name}</p>}
      </div>

      <div className="section">
        <label className="section-label">
          Number of rows
          <span className="hint"> (leave blank for all)</span>
        </label>
        <input
          type="number"
          className="row-input"
          placeholder="e.g. 100"
          value={rowLimit}
          onChange={e => setRowLimit(e.target.value)}
          min={1}
        />
      </div>

      <div className="section">
        <label className="section-label">
          Select checks to run
          <span className="hint"> (untick all = run everything)</span>
        </label>
        <div className="check-grid">
          {CHECKS.map(check => (
            <label key={check.id} className="check-item">
              <input
                type="checkbox"
                checked={selectedChecks.includes(check.id)}
                onChange={() => toggleCheck(check.id)}
              />
              {check.label}
            </label>
          ))}
        </div>
      </div>

      {error && <p className="error-msg">{error}</p>}

      <button className="run-btn" onClick={handleSubmit} disabled={loading}>
        {loading ? "Running..." : "Run validation"}
      </button>
    </div>
  );
}