using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Data.Interfaces;
using SK.GraphRag.Application.Services;
using SK.GraphRag.Application.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.GraphRag.Application.Data;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
public class Neo4jDataAccess : INeo4jDataAccess
{
    private readonly string _database;

    private readonly IAsyncSession _session;

    private readonly ILogger<Neo4jDataAccess> _logger;

    private static readonly Action<Microsoft.Extensions.Logging.ILogger, string, Exception?> _logQueryError =
    LoggerMessage.Define<string>(LogLevel.Error, 
        new EventId(1, nameof(Neo4jDataAccess)),
        "There was a problem while executing database {AccessType} query");

    public Neo4jDataAccess(
        IDriver driver,
        IOptions<GraphDatabaseSettings> options,
        ILogger<Neo4jDataAccess> logger)
    {
        ArgumentNullException.ThrowIfNull(driver);
        ArgumentNullException.ThrowIfNull(options);

        // TODO: Pass db into methods from callers or inject/make generic/generic base
        _database = options.Value.MoviesDb ?? "neo4j";
        _session = driver.AsyncSession(o => o.WithDatabase(_database));
        _logger = logger;
    }

    public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync<Dictionary<string, object>>(query, returnObjectKey, parameters);
    }

    public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
    }

    public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.ReadTransactionAsync(async tx =>
            {
                T scalar = default(T);

                var res = await tx.RunAsync(query, parameters);

                scalar = (await res.SingleAsync())[0].As<T>();

                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logQueryError(_logger, "Read Scalar", ex);
            throw;
        }
    }

    public async Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.WriteTransactionAsync(async tx =>
            {
                T scalar = default(T);

                var res = await tx.RunAsync(query, parameters);

                scalar = (await res.SingleAsync())[0].As<T>();

                return scalar;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logQueryError(_logger, "Write", ex);
            throw;
        }
    }

    private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters)
    {
        try
        {
            parameters = parameters == null ? new Dictionary<string, object>() : parameters;

            var result = await _session.ReadTransactionAsync(async tx =>
            {
                var data = new List<T>();

                var res = await tx.RunAsync(query, parameters);

                var records = await res.ToListAsync();

                data = records.Select(x => (T)x.Values[returnObjectKey]).ToList();

                return data;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logQueryError(_logger, "Read", ex);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _session.CloseAsync();
        await _session.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
