using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace _Game.Scripts.Infrastructure
{
    /// <summary>
    /// Handler untuk operasi baca/tulis data CSV dengan fitur:
    /// - Serialization/deserialization object
    /// - Culture-aware formatting
    /// - Escape karakter khusus CSV
    /// - Error handling robust
    /// </summary>
    public class CsvDataHandler
    {
        #region Fields and Constructor
        
        private readonly string _filePath;
        private readonly CultureInfo _cultureInfo;

        /// <summary>
        /// Membuat instance CsvDataHandler
        /// </summary>
        /// <param name="filePath">Path lengkap ke file CSV</param>
        /// <param name="cultureInfo">Culture info untuk formatting (default: invariant culture)</param>
        public CsvDataHandler(string filePath, CultureInfo cultureInfo = null)
        {
            _filePath = filePath;
            _cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Menulis data ke file CSV
        /// </summary>
        /// <typeparam name="T">Tipe data object</typeparam>
        /// <param name="data">Koleksi data yang akan ditulis</param>
        /// <param name="append">Mode append ke file existing (default: false)</param>
        /// <exception cref="IOException">Gagal menulis ke file</exception>
        public void WriteData<T>(IEnumerable<T> data, bool append = false)
        {
            // Validasi input
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            var csv = new StringBuilder();
            var dataList = data.ToList();

            // Generate header jika file baru atau tidak append
            if (!File.Exists(_filePath) || !append)
            {
                csv.AppendLine(CreateHeader<T>());
            }

            // Generate seluruh rows
            foreach (var item in dataList)
            {
                csv.AppendLine(CreateRow(item));
            }

            // Tulis ke file dengan encoding UTF-8
            try
            {
                File.AppendAllText(_filePath, csv.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write to CSV file: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Membaca data dari file CSV
        /// </summary>
        /// <typeparam name="T">Tipe data object output</typeparam>
        /// <returns>List of deserialized objects</returns>
        /// <exception cref="FileNotFoundException">File tidak ditemukan</exception>
        public List<T> ReadData<T>() where T : new()
        {
            // Validasi file exist
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("CSV file not found", _filePath);

            var lines = File.ReadAllLines(_filePath, Encoding.UTF8);
            
            // Handle empty file
            if (lines.Length == 0)
                return new List<T>();

            // Parse header dan rows
            var headers = ParseLine(lines[0]);
            return lines
                .Skip(1) // Lewati header
                .Select(line => CreateObjectFromRow<T>(headers, ParseLine(line)))
                .ToList();
        }

        #endregion

        #region Helper Methods - CSV Construction

        /// <summary>
        /// Membuat header CSV dari properti object
        /// </summary>
        private string CreateHeader<T>()
        {
            var properties = GetAccessibleProperties<T>();
            return string.Join(",", properties.Select(p => EscapeCsv(p.Name)));
        }

        /// <summary>
        /// Membuat baris CSV dari sebuah object
        /// </summary>
        private string CreateRow<T>(T item)
        {
            var properties = GetAccessibleProperties<T>();
            var rowValues = new List<string>(properties.Length);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(item) ?? "";
                rowValues.Add(EscapeCsv(Convert.ToString(value, _cultureInfo)));
            }

            return string.Join(",", rowValues);
        }

        #endregion

        #region Helper Methods - CSV Parsing

        /// <summary>
        /// Memproses satu baris CSV dengan handling:
        /// - Quote escaping
        /// - Comma dalam field
        /// - Newline dalam field
        /// </summary>
        /// <param name="line">Raw CSV line</param>
        /// <returns>Array of parsed values</returns>
        private string[] ParseLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder(line.Length);
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // Handle quote escaping
                if (c == '"' && (i == 0 || line[i-1] != '\\'))
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                // Handle comma separator di luar quotes
                if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            // Tambahkan field terakhir
            result.Add(current.ToString());
            
            // Unescape dan kembalikan hasil
            return result.Select(UnescapeCsv).ToArray();
        }

        /// <summary>
        /// Membuat object dari parsed CSV row
        /// </summary>
        private T CreateObjectFromRow<T>(string[] headers, string[] values) where T : new()
        {
            var obj = new T();
            var properties = GetAccessibleProperties<T>();

            foreach (var prop in properties)
            {
                // Cari index header yang sesuai dengan nama property
                int headerIndex = Array.FindIndex(
                    headers, 
                    h => h.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (headerIndex >= 0 && headerIndex < values.Length)
                {
                    var convertedValue = ConvertStringToType(values[headerIndex], prop.PropertyType);
                    prop.SetValue(obj, convertedValue);
                }
            }

            return obj;
        }

        #endregion

        #region Type Conversion

        /// <summary>
        /// Konversi string ke tipe target dengan handling:
        /// - Null values
        /// - Culture-specific formatting
        /// - Enum values
        /// - Nullable types
        /// </summary>
        private object ConvertStringToType(string value, Type targetType)
        {
            // Handle empty string
            if (string.IsNullOrWhiteSpace(value))
                return GetDefaultValue(targetType);

            try
            {
                // Handle string langsung
                if (targetType == typeof(string))
                    return UnescapeCsv(value);

                // Handle enum
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);

                // Handle nullable types
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return Convert.ChangeType(UnescapeCsv(value), underlyingType, _cultureInfo);
            }
            catch
            {
                // Fallback ke default value jika konversi gagal
                return GetDefaultValue(targetType);
            }
        }

        /// <summary>
        /// Mendapatkan default value untuk suatu type
        /// </summary>
        private object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Mendapatkan properti yang bisa diakses (public read/write)
        /// </summary>
        private PropertyInfo[] GetAccessibleProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();
        }

        /// <summary>
        /// Escape karakter khusus CSV
        /// </summary>
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            
            if (value.Contains("\"") || value.Contains(",") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        /// <summary>
        /// Membersihkan escape characters dari nilai CSV
        /// </summary>
        private string UnescapeCsv(string value)
        {
            value = value.Trim();

            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value[1..^1] // Remove surrounding quotes
                    .Replace("\"\"", "\""); // Unescape inner quotes
            }

            return value;
        }

        #endregion
    }
}