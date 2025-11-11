using System.Data;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Inventory.Context;

public class ADODbContext
{
    private NpgsqlConnection _connection;

    public ADODbContext(IConfiguration configuration)
    {
        _connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultString"));

    }

    private async Task OpenConnectionAsync()
    {
        await _connection.OpenAsync();
    }

    private async Task CloseConnectionAsync()
    {
        await _connection.CloseAsync();
    }

    public async Task<T?> ExecuteQueryGetObject<T>(string query, Dictionary<string, object>? parameters = null)
    {

        NpgsqlCommand cmd = new NpgsqlCommand(query, _connection);

        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
        }

        try
        {
            await OpenConnectionAsync();

            var reader = await cmd.ExecuteReaderAsync();

            var list = DataReaderToList<T>(reader);
            if (list.Count() > 0)
            {
                return list[0];
            }

            return default(T);
        }
        finally
        {
            await CloseConnectionAsync();
        }

    }

    public async Task<List<T?>> ExecuteQueryGetList<T>(string query, Dictionary<string, object>? parameters = null)
    {

        NpgsqlCommand cmd = new NpgsqlCommand(query, _connection);

        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
        }

        try
        {
            await OpenConnectionAsync();

            var reader = await cmd.ExecuteReaderAsync();

            var list = DataReaderToList<T>(reader);
            if (list.Count() > 0)
            {
                return list;
            }

            return [];
        }
        finally
        {
            await CloseConnectionAsync();
        }

    }

    public async Task<T> ExecuteQuery<T>(string query, Dictionary<string, object>? parameters = null)
    {

        NpgsqlCommand cmd = new NpgsqlCommand(query, _connection);

        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
        }

        try
        {
            await OpenConnectionAsync();

            var result = await cmd.ExecuteScalarAsync();

            return (T)result ?? default(T);
        }
        finally
        {
            await CloseConnectionAsync();
        }
    }

    private static List<T> DataReaderToList<T>(IDataReader dr)
    {
        List<T> list = new List<T>();

        T obj = default(T);

        while (dr.Read())
        {
            obj = Activator.CreateInstance<T>();

            for (int i = 0; i < dr.FieldCount; i++)
            {
                PropertyInfo info = obj.GetType().GetProperties().FirstOrDefault(o => o.Name.ToLower() == dr.GetName(i).ToLower());
                if (info != null)
                {
                    info.SetValue(obj, dr.GetValue(i) != System.DBNull.Value ? dr.GetValue(i) : null, null);
                }
            }
            list.Add(obj);
        }

        return list;
    }
}
