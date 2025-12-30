using System.Text.Json.Serialization;
using Inventory.Common.Helpers;

namespace Inventory.Models.Entities;

public class ErrorLog : BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }

    public string EncryptedId => EncryptionHelper.EncryptId(Id.ToString());

    public DateTime LogDate { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    public string? StackTrace { get; set; }
}
