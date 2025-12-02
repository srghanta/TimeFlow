using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeFlow.Data;
using TimeFlow.Models;

namespace TimeFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // GET Attendance
    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(DateTime? date = null)
    {
        var query = _db.Attendance
            .Include(a => a.User)
            .AsQueryable();

        if (date.HasValue)
        {
            var d = date.Value.Date;
            query = query.Where(a => a.CheckInTime.Date == d);
        }

        var results = await query
            .OrderBy(a => a.User.Username)
            .ThenBy(a => a.CheckInTime)
            .ToListAsync();

        return Ok(results);
    }

    // GET Users list
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .OrderBy(u => u.Username)
            .ToListAsync();

        return Ok(users);
    }

    // CREATE User
    public record CreateUserRequest(string Username, string Password, string Role);

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            return BadRequest("Username already exists.");

        var user = new User
        {
            Username = request.Username,
            Role = request.Role
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }

    // UPDATE User
    public record UpdateUserRequest(string Username, string Role);

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Username = request.Username;
        user.Role = request.Role;

        await _db.SaveChangesAsync();
        return Ok(user);
    }

    // RESET Password
    public record ResetPasswordRequest(string NewPassword);

    [HttpPut("users/{id}/password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

        await _db.SaveChangesAsync();
        return Ok("Password reset successfully.");
    }

    // DELETE User
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return Ok("User deleted.");
    }
}
