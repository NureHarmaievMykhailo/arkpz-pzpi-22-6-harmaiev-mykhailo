namespace RoadMonitoringSystem.DTO
{
    public class SensorDto
    {
        public int RoadSectionID { get; set; } 
        public string SensorType { get; set; }
        public DateTime InstallationDate { get; set; }
        public string Status { get; set; }
    }
}
