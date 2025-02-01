namespace RoadMonitoringSystem.DTO
{
    public class CreateRoadSectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
