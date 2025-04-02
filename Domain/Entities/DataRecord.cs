namespace Domain.Entities
{
    public class DataRecord
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public string? OriginalTime { get; set; }
    }
}
