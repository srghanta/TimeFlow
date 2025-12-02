using Microsoft.EntityFrameworkCore;
using TimeFlow.Data;

namespace TimeFlow.Services;

public class DailyLogoutService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DailyLogoutService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var runAt = DateTime.Today.AddDays(1).AddSeconds(-1); // 18:59:59
            var delay = runAt - now;
            if (delay.TotalMilliseconds <= 0) delay = TimeSpan.FromSeconds(10);

            await Task.Delay(delay, stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var active = await db.Attendance.Where(a => a.CheckOutTime == null).ToListAsync(stoppingToken);
            foreach (var a in active)
            {
                a.CheckOutTime = DateTime.Today.AddSeconds(86399); // 18:59:59
                a.IsAutoLoggedOut = true;
            }

            await db.SaveChangesAsync(stoppingToken);
        }
    }
}
