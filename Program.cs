using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FirebirdSql.Data.FirebirdClient;

namespace FirebirdExample;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new ContainerBuilder();

// db schema
/*
create table TMP (TMP_ID bigint, VAL VARCHAR(10));

insert into TMP (TMP_ID, VAL) values ('1', 'mary' );
insert into TMP (TMP_ID, VAL) values ('2', 'fuck' );
insert into TMP (TMP_ID, VAL) values ('3', 'kill' );
*/
        builder.Register<FbConnectionStringBuilder>(ctx =>
        {
            return new FbConnectionStringBuilder
            {
                DataSource = "127.0.0.1",
                Port = 3050,
                UserID = "SYSDBA",
                Password = "masterkey",
                Database = "/firebird/data/TMP.FDB",
                ServerType = FbServerType.Default,
                Charset = "UTF8",
                Pooling = true,
                MaxPoolSize = 3
            };
        });
        builder.RegisterType<FirebirdReadTransactionFactory>();

        builder.RegisterType<DbQueryExample>().AsImplementedInterfaces();
        builder.RegisterType<DbQueryExample>().AsImplementedInterfaces();
        builder.RegisterType<DbQueryExample>().AsImplementedInterfaces();
        builder.RegisterType<DbQueryExample>().AsImplementedInterfaces();
        builder.RegisterType<DbQueryExample>().AsImplementedInterfaces();
        var container = builder.Build();

        try
        {
            var parallelQueries = container.Resolve<IEnumerable<IAsyncExecution>>();
            await ExecuteAllAsync(parallelQueries);
        }
        catch (Exception ex)
        {
            Console.WriteLine("we had an error - dont close app");
            Console.ReadLine();
        }
        
        Console.WriteLine("fin.");
        Console.ReadLine();
    }

    private static async Task ExecuteAllAsync(IEnumerable<IAsyncExecution> services)
    {
        var tasks = services.Select(s => s.Execute());
        await Task.WhenAll(tasks.ToArray());
    }
}
