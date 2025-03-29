using System;
using System.Collections.Generic;
using System.Text;

namespace VLExtensions
{
    /// <summary>
    /// Parses comma separated values (csv). e.g. from files exported from Excel or Sheets.
    /// </summary>
    public static class CSVParser
    {
        /// <summary>
        /// Parses the columns and rows from the <paramref name="csv"/> string.
        /// Rows are separated by newline \n or \r\n (\r only is not supported).
        /// Columns are separated by <paramref name="separator"/>.
        /// Values that are surrounded with "double quotes" can contain the separators.
        /// This behaviour should be compatible with csv files exported from Excel or Sheets.
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="separator"></param>
        /// <returns>List of columns. data[col][row]</returns>
        public static List<List<string>> Parse(ReadOnlySpan<char> csv, char separator = ',')
        {
            if (separator == '\n' || separator == '\r' || separator == '"')
                throw new ArgumentException($"Separator not allowed", nameof(separator));

            List<List<string>> data = new();

            static void SetValue(List<List<string>> data, int row, int col, string value)
            {
                while (data.Count <= col)
                    data.Add(new());
                while (data[col].Count <= row)
                    data[col].Add(string.Empty);

                data[col][row] = value;
            }

            StringBuilder sb = new();
            int row = 0;
            int col = 0;
            bool isQuoted = false;
            char lastChar = default;
            for (int i = 0; i < csv.Length; i++)
            {
                if (csv[i] == '"')
                {
                    isQuoted = !isQuoted;
                    if (isQuoted && lastChar == '"')
                    {
                        sb.Append('"');
                    }
                }
                else if (csv[i] == separator)
                {
                    if (!isQuoted)
                    {
                        SetValue(data, row, col, sb.ToString());
                        sb.Length = 0;
                        col++;
                    }
                    else
                    {
                        sb.Append(csv[i]);
                    }
                }
                else if (csv[i] == '\r')
                {
                    if (!isQuoted)
                    {
                        // TODO: handle files with only \r newlines
                    }
                    else
                    {
                        sb.Append(csv[i]);
                    }
                }
                else if (csv[i] == '\n')
                {
                    if (!isQuoted)
                    {
                        SetValue(data, row, col, sb.ToString());
                        sb.Length = 0;
                        row++;
                        col = 0;
                    }
                    else
                    {
                        sb.Append(csv[i]);
                    }
                }
                else
                {
                    sb.Append(csv[i]);
                }
                lastChar = csv[i];
            }

            SetValue(data, row, col, sb.ToString());
            sb.Length = 0;

            return data;
        }

        private static readonly char[] CharsToEscape = { '\n', '\r', '"' };

        /// <summary>
        /// Places value inside double quotes if it contains characters that need to be escaped.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <param name="escapeNewLines">Replaces new lines with \r and \n.</param>
        /// <returns></returns>
        public static string Escape(string value, char separator = ',', bool escapeNewLines = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            if (escapeNewLines)
                value = value.Replace("\r", "\\r").Replace("\n", "\\n");

            if (value.IndexOf(separator, StringComparison.Ordinal) != -1
             || value.IndexOfAny(CharsToEscape) != -1)
                return $"\"{value.Replace("\"", "\"\"")}\"";
            else
                return value;
        }
    }
}
