using Microsoft.Extensions.AI;
using SK.GraphRag.Application.Data.Interfaces;

namespace SK.GraphRag.Application.EinsteinQuery.Interfaces;

public interface IEinsteinQueryDataAccess : INeo4jDataAccess
{
    Task CreateFullTextIndexIfNotExists();

    Task CreateChildVectorIndexIfNotExists();

    Task CreateChunkVectorIndexIfNotExists();

    Task<IList<RankedSearchResult>> QuerySimilarRecords(ReadOnlyMemory<float> queryEmbedding, int k = 3);

    Task SaveTextChunks(IReadOnlyList<string> chunks, IReadOnlyList<ReadOnlyMemory<float>> embeddings, int startIndex = 0);

    Task RemoveExistingData();
}
