namespace api.Storage;

using Azure;
using Azure.Data.Tables;
public class TableUserEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "USER";
    public string RowKey { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
