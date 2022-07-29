using System;
using System.Data;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace FirebirdExample;

public class FirebirdReadTransactionFactory
{
    private readonly FbConnectionStringBuilder _fbConnectionStringBuilder;

    public FirebirdReadTransactionFactory(
        FbConnectionStringBuilder fbConnectionStringBuilder)
    {
        _fbConnectionStringBuilder = fbConnectionStringBuilder;
    }

    public async Task<T> Execute<T>(Func<IDbTransaction, Task<T>> func)
    {
        using var connection = await GetAsyncConnection(_fbConnectionStringBuilder);
        using var transaction = connection.BeginTransaction(DbTransactionOptions.Read);
        var result = await func(transaction);
        transaction.Commit();
        return result;
    }

    private static async Task<FbConnection> GetAsyncConnection(FbConnectionStringBuilder connectionBuilder)
    {
        var connection = new FbConnection(connectionBuilder.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
    
    private class DbTransactionOptions
    {
        public static FbTransactionOptions Read => new()
        {
            TransactionBehavior = FbTransactionBehavior.Read |
                                  FbTransactionBehavior.RecVersion |
                                  FbTransactionBehavior.Wait |
                                  FbTransactionBehavior.ReadCommitted,
            WaitTimeout = TimeSpan.FromMinutes(2),
        };
    }
}

