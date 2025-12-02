namespace TimeFlow.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User";

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}
