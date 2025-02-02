namespace RoadMonitoringSystem.DTO
{
    public class SensorDataDto
    {
        public int SensorID { get; set; }
        public string Parameter { get; set; }
        public double DataValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
