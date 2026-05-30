using System;
using System.IO;

namespace Transaction.Pipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = @"Database\data\raw\paysim.csv";
            string outputPath =  @"Database\data\raw\paysim_01.csv";
            int rowLimit = 100;

            Console.WriteLine("Reading CSV file...");

            string[] allLines = File.ReadAllLines(inputPath);

            string header =allLines[0];

            string[] sampleLines = new string[rowLimit + 1];
            sampleLines[0] = header;

            for (int i = 1; i <=rowLimit; i++)
            {
                sampleLines[i] = allLines[i];
            }

            File.WriteAllLines(outputPath, sampleLines);

            Console.WriteLine($"Done - {rowLimit} rows saved to {outputPath}");
        }
    }
}
