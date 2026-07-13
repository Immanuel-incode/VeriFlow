import { useState } from "react";
import PipelineForm from "./components/PipelineForm";
import ValidationReport from "./components/ValidationReport";
import CleaningReport from "./components/CleaningReport";
import EnrichmentReport from "./components/EnrichmentReport";
require("./App.css");

export default function App() {

  const [results, setResults] = useState<any>(null);

  return (
    <div className="app">

      <header className="app-header">
        <h1>VeriFlow</h1>
        <p>
          Choose an operation and run the pipeline.
        </p>
      </header>

      <main className="app-main">

        <PipelineForm onResults={setResults} />

        {results && results.operation === "validation" && (
          <ValidationReport data={results} />
        )}

        {results && results.operation === "cleaning" && (
          <CleaningReport data={results} />
        )}

        {results && results.operation === "enrichment" && (
          <EnrichmentReport data={results} />
        )}

      </main>

    </div>
  );
}