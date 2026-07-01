interface Props {
  data: any;
}

// Helper — renders one error category
function ErrorSection({ label, errors }: { label: string; errors: string[] }) {
  return (
    <div className="error-section">
      <div className="error-header">
        <span className="error-label">{label}</span>
        <span className={`error-count ${errors.length > 0 ? "has-errors" : "no-errors"}`}>
          {errors.length} error{errors.length !== 1 ? "s" : ""}
        </span>
      </div>
      {errors.length > 0 && (
        <ul className="error-list">
          {errors.map((e, i) => (
            <li key={i}>{e}</li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default function ValidationReport({ data }: Props) {
  const { summary } = data;

  return (
    <div className="report-card">
      <h2>Validation report</h2>

      {/* Summary KPI cards */}
      <div className="kpi-row">
        <div className="kpi kpi-total">
          <div className="kpi-value">{summary.total_rows}</div>
          <div className="kpi-label">Total rows</div>
        </div>
        <div className="kpi kpi-pass">
          <div className="kpi-value">{summary.passed_rows}</div>
          <div className="kpi-label">Passed</div>
        </div>
        <div className="kpi kpi-error">
          <div className="kpi-value">{summary.total_errors}</div>
          <div className="kpi-label">Errors found</div>
        </div>
      </div>

      {/* Error breakdown */}
      <h3>Error breakdown</h3>
      <ErrorSection label="Completeness"   errors={data.completeness_errors} />
      <ErrorSection label="Data type"      errors={data.datatype_errors} />
      <ErrorSection label="Binary"         errors={data.binary_errors} />
      <ErrorSection label="Format"         errors={data.format_errors} />
      <ErrorSection label="Allowed values" errors={data.allowed_value_errors} />
      <ErrorSection label="Consistency"    errors={data.consistency_errors} />
    </div>
  );
}