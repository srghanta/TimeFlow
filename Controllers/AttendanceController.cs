using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeFlow.Services;


namespace TimeFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _attendanceService;

    public AttendanceController(AttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new Exception("User ID claim missing.");
        return int.Parse(idClaim);
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn()
    {
        var userId = GetUserId();
        var record = await _attendanceService.CheckInAsync(userId);
        return Ok(record);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut()
    {
        var userId = GetUserId();
        var record = await _attendanceService.CheckOutAsync(userId);
        return Ok(record);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History(DateTime? from = null, DateTime? to = null)
    {
        var userId = GetUserId();
        var records = await _attendanceService.GetUserHistoryAsync(userId, from, to);
        return Ok(records);
    }
}
