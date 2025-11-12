using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Data;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;
using SK.GraphRag.Application.Settings;
using System.Globalization;

namespace SK.GraphRag.Application.EinsteinQuery;

public class EinsteinDataAccess : Neo4jDataAccess, IEinsteinQueryDataAccess
{
    private readonly ILogger<EinsteinDataAccess> _logger;

#pragma warning disable CA1848 // Use the LoggerMessage delegates - can remove this when all logging is moved to delegates
#pragma warning disable IDE0290 // Use primary constructor - disabled because of code passing options base class constructor call
    public EinsteinDataAccess(
        IDriver driver,
        IOptions<GraphDatabaseSettings> options,
        ILogger<EinsteinDataAccess> logger)
        : base(
            driver,
            options?.Value?.EinsteinVectorDb ?? GraphDatabaseSettings.DefaultDb,
            logger)
    {
        _logger = logger;
    }
#pragma warning restore IDE0290 // Use primary constructor

    public async Task CreateFullTextIndexIfNotExists()
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await ExecuteWriteTransactionAsync(
                """
                CREATE FULLTEXT INDEX index_ftPdfChunk IF NOT EXISTS
                FOR (c:Chunk) 
                ON EACH [c.text]
                """).ConfigureAwait(false);

            _logger.LogInformation(@"Created full-text index.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating full-text index: {Message}", ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public async Task CreateChunkVectorIndexIfNotExists()
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await ExecuteWriteTransactionAsync(
                """
                CREATE VECTOR INDEX index_pdfChunk IF NOT EXISTS 
                FOR (c:Chunk)
                ON c.embedding
                """).ConfigureAwait(false);

            _logger.LogInformation("Created vector index 'index_pdfChunk'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating vector index: {Message}", ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public async Task CreateChildVectorIndexIfNotExists()
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await ExecuteWriteTransactionAsync(
                """
                CREATE VECTOR INDEX index_parent IF NOT EXISTS
                FOR (c:Child)
                ON c.embedding
                """).ConfigureAwait(false);

            _logger.LogInformation("Created vector index 'index_parent'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating vector index: {Message}", ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public async Task<IList<RankedSearchResult>> QuerySimilarRecords(ReadOnlyMemory<float> queryEmbedding, int k = 3)
    {
        List<RankedSearchResult> rankedResults = [];

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var results = await ExecuteReadDictionaryAsync(
                """
                CALL db.index.vector.queryNodes('index_pdfChunk', $k, $question_embedding) 
                YIELD node AS hits, score
                RETURN hits{ text: hits.text, score, index: hits.index } AS rankedResult
                ORDER BY score DESC
                """,
                "rankedResult",
                new Dictionary<string, object>
                {
                    { "k", 2 }, // k as in in KNN - number of nearest neighbors
                    { "question_embedding", queryEmbedding.ToArray() }
                })
                .ConfigureAwait(false);

            rankedResults.AddRange(results.Select(x =>
                new RankedSearchResult(
                    x["text"] as string ?? string.Empty,
                    Convert.ToDouble(x["score"], CultureInfo.InvariantCulture),
                    Convert.ToInt32(x["index"], CultureInfo.InvariantCulture)
                )));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while querying similar records: {Message}", ex.Message);
        }

        return rankedResults;
#pragma warning restore CA1031 // Do not catch general exception types
    }

    public async Task RemoveExistingData()
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await ExecuteWriteTransactionAsync("DROP INDEX index_ftPdfChunk IF EXISTS;").ConfigureAwait(false);
            await ExecuteWriteTransactionAsync("DROP INDEX index_pdfChunk IF EXISTS;").ConfigureAwait(false);
            await ExecuteWriteTransactionAsync("DROP INDEX index_parent IF EXISTS; ").ConfigureAwait(false);

            await ExecuteWriteTransactionAsync("MATCH (c:Chunk) DETACH DELETE c;").ConfigureAwait(false);
            await ExecuteWriteTransactionAsync("MATCH (c:Child) DETACH DELETE c;").ConfigureAwait(false);
            await ExecuteWriteTransactionAsync("MATCH (p:Parent) DETACH DELETE p;").ConfigureAwait(false);

            _logger.LogInformation(@"Removed existing data and indexes from database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating full-text index: {Message}", ex.Message);
        }
    }

    public async Task SaveTextChunks(IReadOnlyList<string> chunks, IReadOnlyList<ReadOnlyMemory<float>> embeddings, int startIndex = 0)
    {
        if (chunks is null || embeddings is null || chunks.Count != embeddings.Count)
        {
            throw new ArgumentException("Chunks and embeddings must be non-null and have the same count.");
        }

        if (chunks.Count == 0 || embeddings.Count == 0)
        {
            _logger.LogWarning("No chunks or embeddings to save.");
            return;
        }

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await ExecuteWriteTransactionAsync(
                """
                WITH $chunks as chunks, range(0, size($chunks) - 1) AS index
                UNWIND index AS i
                WITH i, chunks[i] AS chunk, $embeddings[i] AS embedding
                MERGE (c:Chunk {index: i + $startIndex})
                SET c.text = chunk, c.embedding = embedding
                """,
                new Dictionary<string, object>
                {
                    { "chunks", chunks },
                    { "embeddings", embeddings.Select(e => e.ToArray()).ToList() },
                    { "startIndex", startIndex }
                })
                .ConfigureAwait(false);

            _logger.LogInformation("saving {ChunkCount} text chunks and {EmbeddingCount} embeddings.", chunks.Count, embeddings.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving text chunks and embeddings: {Message}", ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

#pragma warning restore CA1848 // Use the LoggerMessage delegates
}
