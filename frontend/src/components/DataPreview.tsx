interface Props {
  rows: any[];
  columns?: string[];
  maxRows?: number;
}
export default function DataPreview({
  rows,
  columns,
  maxRows = 10,
}: Props) {
//Displays a message if there is no data
  if (!rows || rows.length === 0) {
    return <p>No data available.</p>;
  }
//Retrieve the column names from the first row
  const displayColumns = columns ?? Object.keys(rows[0]);
  return (
    <div>
      <h3>Dataset Preview</h3>
      <table className="preview-table">
        <thead>
          <tr>
            {displayColumns.map((column) => (
              <th key={column}>{column}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows
            .slice(0, maxRows)
            .map((row, rowIndex) => (
              <tr key={rowIndex}>
                {displayColumns.map((column) => (
                  <td key={column}>
                    {String(row[column])}
                  </td>
                ))}
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
}