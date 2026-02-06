using DTO.Dashboard;

namespace Business.Interface;

public interface IDashboardService
{
    Task<AdminDashboardDTO> GetAdminDashboardStatsAsync();
}
