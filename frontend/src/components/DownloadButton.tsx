interface Props {
  rows: any[];
  filename: string;
}
export default function DownloadButton({
  rows,
  filename,
}: Props) {
//Downloads the processed dataset as a CSV file
  const downloadCSV = () => {
//Does nothing if there is no data
    if (!rows || rows.length === 0) {
      alert("No data available to download.");
      return;
    }
//Retrieves the column names
    const headers = Object.keys(rows[0]);
//Builds the CSV rows
    const csv = [
      headers.join(","),
      ...rows.map((row) =>
        headers.map((header) => {
        const value = row[header] ?? "";
        return `"${String(value).replace(/"/g, '""')}"`;
    })
    .join(",")
      ),
    ].join("\n");
//Creates a downloadable file
    const blob = new Blob([csv], {
      type: "text/csv;charset=utf-8;",
    });
    const url = window.URL.createObjectURL(blob);
//Creates a temporary download link
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };
  return (
    <button
      type="button"
      onClick={downloadCSV}
      style={{
        marginTop: "20px",
      }}
    >
      Download CSV
    </button>
  );
}