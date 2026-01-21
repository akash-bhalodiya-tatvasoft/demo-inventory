using System.Data;

namespace Inventory.Context;

public interface IADODbContext
{
    Task<T?> ExecuteQueryGetObject<T>(
        string query,
        Dictionary<string, object>? parameters = null
    );

    Task<List<T?>> ExecuteQueryGetList<T>(
        string query,
        Dictionary<string, object>? parameters = null
    );

    Task<T> ExecuteQuery<T>(
        string query,
        Dictionary<string, object>? parameters = null
    );

    Task ExecuteProcedure(
        string procedureName,
        Dictionary<string, object>? parameters = null
    );
}
