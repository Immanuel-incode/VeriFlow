import DataPreview from "./DataPreview";
import DownloadButton from "./DownloadButton";
interface Props {
  data: any;
}
//Maps backend operation names to user friendly labels
const operationLabels: Record<string, string> = {
  risklevel: "Generate Risk Level",
  senderbalancedifference: "Calculate Sender Balance Difference",
  recipientbalancedifference: "Calculate Recipient Balance Difference",
};

export default function EnrichmentReport({ data }: Props) {
  const { summary } = data;
  return (
    <div className="report-card">
      <h2>Enrichment Report</h2>
{/* Summary KPI cards */}
      <div className="kpi-row">
        <div className="kpi kpi-total">
          <div className="kpi-value">
            {summary.total_input}
          </div>
          <div className="kpi-label">
            Input Rows
          </div>
        </div>
        <div className="kpi kpi-pass">
          <div className="kpi-value">
            {summary.total_enriched}
          </div>
          <div className="kpi-label">
            Enriched
          </div>
        </div>
        <div className="kpi kpi-total">
          <div className="kpi-value">
            {summary.operations_run.length}
          </div>
          <div className="kpi-label">
            Operations
          </div>
        </div>
      </div>
      <h3>Enrichment Summary</h3>
      <p>
        <strong>Operations Applied:</strong>
      </p>
      <ul className="error-list">
        {summary.operations_run.map(
          (operation: string, index: number) => (
            <li key={index}>
                {operationLabels[operation]}
            </li>
          )
        )}
      </ul>
      <br />
      <h3>Generated Columns</h3>
      <ul className="error-list">
        {summary.operations_run.includes("risklevel") && (
          <li>riskLevel</li>
        )}
        {summary.operations_run.includes("senderbalancedifference") && (
          <li>senderBalanceDifference</li>
        )}
        {summary.operations_run.includes("recipientbalancedifference") && (
          <li>recipientBalanceDifference</li>
        )}
      </ul>
      <DataPreview rows={data.enriched_transactions}
        columns={["amount", "riskLevel", "senderBalanceDifference", "recipientBalanceDifference"]}
      />
      <DownloadButton rows={data.enriched_transactions} filename="enriched_transactions.csv"/>
    </div>
  );
}
