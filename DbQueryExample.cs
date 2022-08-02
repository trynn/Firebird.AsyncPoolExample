using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace FirebirdExample;

public interface IAsyncExecution
{
    public Task Execute();
}

public class DbQueryExample : IAsyncExecution
{
    private readonly FirebirdReadTransactionFactory _firebirdReadTransactionFactory;

    public DbQueryExample(
        FirebirdReadTransactionFactory firebirdReadTransactionFactory)
    {
        _firebirdReadTransactionFactory = firebirdReadTransactionFactory;
    }
    
    private record TmpQuery(long TMP_ID, string VAL);
    
    public async Task Execute()
    {
        Console.WriteLine("execute load");
        var data = await Load();
        foreach (var item in data)
            Console.WriteLine($"got {item.VAL}");
    }

    private async Task<IEnumerable<TmpQuery>> Load()
    {
        const string sql = @"select * from tmp";
        
        var data = await _firebirdReadTransactionFactory.Execute(async transaction =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: null,
                transaction: transaction,
                commandType: null,
                cancellationToken: default);

            await Task.Delay(1000);
            return await transaction.Connection.QueryAsync<TmpQuery>(command);
        });

        return data;
    }
}