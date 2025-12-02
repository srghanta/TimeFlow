namespace TimeFlow.Models;

public class Attendance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public bool IsAutoLoggedOut { get; set; }
}
