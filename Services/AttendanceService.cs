using Microsoft.EntityFrameworkCore;
using TimeFlow.Data;
using TimeFlow.Models;

namespace TimeFlow.Services;

public class AttendanceService
{
    private readonly AppDbContext _db;

    public AttendanceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Attendance?> GetOpenRecordAsync(int userId)
    {
        return await _db.Attendance
            .Where(a => a.UserId == userId && a.CheckOutTime == null)
            .OrderByDescending(a => a.CheckInTime)
            .FirstOrDefaultAsync();
    }

    public async Task<Attendance> CheckInAsync(int userId)
    {
        var open = await GetOpenRecordAsync(userId);
        if (open != null)
            throw new Exception("User is already checked in.");

        var record = new Attendance
        {
            UserId = userId,
            CheckInTime = DateTime.UtcNow
        };

        _db.Attendance.Add(record);
        await _db.SaveChangesAsync();
        return record;  //  MUST RETURN
    }

    public async Task<Attendance> CheckOutAsync(int userId)
    {
        var open = await GetOpenRecordAsync(userId);
        if (open == null)
            throw new Exception("No open session to check out.");

        open.CheckOutTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return open;  //  MUST RETURN
    }

    public async Task<IEnumerable<Attendance>> GetUserHistoryAsync(int userId, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.Attendance.Where(a => a.UserId == userId);

        if (from.HasValue)
            query = query.Where(a => a.CheckInTime >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.CheckInTime <= to.Value);

        return await query
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();  //  MUST RETURN IEnumerable
    }
}
