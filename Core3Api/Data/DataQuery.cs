using Microsoft.EntityFrameworkCore;
using Models.MyDbContext;
public class DataQuery : IDataQuery
{
    private readonly MyDBContext _dbContext;
    public DataQuery(MyDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<T>> DataListReturn<T>(string procName)
    {
        var cmd = _dbContext.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"EXEC dbo.{procName}";
        await _dbContext.Database.OpenConnectionAsync();
        var reader = await cmd.ExecuteReaderAsync();
        List<T> responses = new List<T>();
        if (reader.HasRows)
        {
            // Đọc từng dòng tập kết quả
            while (await reader.ReadAsync())
            {
                var obj = Activator.CreateInstance<T>();
                var properties = obj.GetType().GetProperties();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                    if (property != null && reader[i] != DBNull.Value)
                    {
                        var value = Convert.ChangeType(reader[i], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                }
                responses.Add(obj);
            }
        }
        await _dbContext.Database.CloseConnectionAsync();
        return responses;
    }

    public async Task<T> DataValueReturn<T>(string procName)
    {
        var cmd = _dbContext.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"EXEC dbo.{procName}";
        await _dbContext.Database.OpenConnectionAsync();
        var value = await cmd.ExecuteScalarAsync();
        await _dbContext.Database.CloseConnectionAsync();
        if (value != null)
            return (T)Convert.ChangeType(value, typeof(T));
        return default(T);
    }
    public void DataNotReturn(string procName)
    {
        var cmd = _dbContext.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"EXEC dbo.{procName}";
        _dbContext.Database.OpenConnection();
        cmd.ExecuteNonQuery();
        _dbContext.Database.CloseConnection();
    }

}