using CsvHelper.Configuration;
using Domain.Entities;
using System.Globalization;

namespace Infrastructure.Helpers
{
    public sealed class DataRecordMap : ClassMap<DataRecord>
    {
        public DataRecordMap()
        {
            Map(m => m.OriginalTime).Convert(row => row.Row.GetField(0)); // Map the original date string
            Map(m => m.Time).Convert(row =>
            {
                var rawValue = row.Row.GetField(0);
                var formats = new[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
                return DateTime.TryParseExact(rawValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
                    ? parsedDate
                    : DateTime.MinValue;
            });
            Map(m => m.Value).Index(1);
        }
    }
}
