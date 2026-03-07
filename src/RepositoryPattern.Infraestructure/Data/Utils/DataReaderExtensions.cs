using System.Data;

namespace RepositoryPattern.Infraestructure.Data.Utils;

public static class DataReaderExtensions
{
    public static int GetInt(this IDataRecord record, string columnName) =>
        record.GetInt32(record.GetOrdinal(columnName));

    public static string? GetString(this IDataRecord record, string columnName)
    {
        var index = record.GetOrdinal(columnName);
        return record.IsDBNull(index) ? null : record.GetString(index);
    }

    public static bool GetBoolean(this IDataRecord record, string columnName)
    {
        var index = record.GetOrdinal(columnName);
        return !record.IsDBNull(index) && record.GetBoolean(index);
    }

    public static DateTime GetDateTime(this IDataRecord record, string columnName)
    {
        var index = record.GetOrdinal(columnName);
        return record.GetDateTime(index);
    }
}