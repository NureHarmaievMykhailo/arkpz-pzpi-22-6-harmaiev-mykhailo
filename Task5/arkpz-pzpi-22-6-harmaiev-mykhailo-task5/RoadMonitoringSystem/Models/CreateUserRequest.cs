namespace RoadMonitoringSystem.Models
{
    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRoles Role { get; set; }
    }
}