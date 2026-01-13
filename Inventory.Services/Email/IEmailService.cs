using Inventory.Models.Entities;

namespace Inventory.Services.Interfaces;

public interface IEmailService
{
    Task SendOrderCreatedEmailAsync(string toEmail, string userName, Order order);
}