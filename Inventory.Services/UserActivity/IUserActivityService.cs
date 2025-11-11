using Inventory.Models.Entities;

namespace Inventory.Services.Interfaces;

public interface IUserActivityService
{
    Task LogAsync(UserActivityLog log);
}
