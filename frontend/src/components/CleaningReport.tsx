interface Props {
  data: any;
}

export default function CleaningReport({ data }: Props) {

  const { summary } = data;

  return (
    <div className="report-card">

      <h2>Cleaning Report</h2>

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
            {summary.total_cleaned}
          </div>
          <div className="kpi-label">
            Cleaned
          </div>
        </div>

        <div className="kpi kpi-error">
          <div className="kpi-value">
            {summary.total_quarantined}
          </div>
          <div className="kpi-label">
            Quarantined
          </div>
        </div>

      </div>

      <h3>Cleaning Summary</h3>

      <p>

        <strong>Duplicates Removed:</strong>

        {" "}

        {summary.duplicates_removed}

      </p>

      <br />

      <h3>Quarantined Rows</h3>

      {

        data.quarantined_rows.length === 0 ?

        <p>No quarantined rows.</p>

        :

        <ul className="error-list">

          {

            data.quarantined_rows.map((row: any, index: number) => (

              <li key={index}>

                Row {row.row}: {row.reason}

              </li>

            ))

          }

        </ul>

      }

    </div>
  );

}