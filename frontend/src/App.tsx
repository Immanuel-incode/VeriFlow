import { useState } from "react";
import PipelineForm from "./components/PipelineForm";
import ValidationReport from "./components/ValidationReport";
require("./App.css");

export default function App() {
  // results holds whatever Python sends back
  // null means nothing has been run yet
  const [results, setResults] = useState<any>(null);

  return (
    <div className="app">
      <header className="app-header">
        <h1>Transaction + Fraud Pipeline</h1>
        <p>Upload a dataset, select checks, and run the pipeline</p>
      </header>

      <main className="app-main">
        {/* Left panel — form */}
        <PipelineForm onResults={setResults} />

        {/* Right panel — results, only shows after running */}
        {results && <ValidationReport data={results} />}
      </main>
    </div>
  );
}