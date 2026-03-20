using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChromaSampleUI.Services;

public class ChromaService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ChromaService(string chromaUrl)
    {
        _baseUrl = chromaUrl.TrimEnd('/');
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    public async Task<string> GetOrCreateCollectionAsync(string name)
    {
        var payload = new { name, get_or_create = true };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/collections", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("id").GetString()!;
    }

    public async Task UpsertDocumentsAsync(string collectionId, List<string> ids, List<string> documents, List<Dictionary<string, string>> metadatas)
    {
        var payload = new
        {
            ids,
            documents,
            metadatas
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/collections/{collectionId}/upsert", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<QueryResult>> QueryAsync(string collectionId, string queryText, int nResults = 5)
    {
        var payload = new
        {
            query_texts = new[] { queryText },
            n_results = nResults,
            include = new[] { "documents", "metadatas", "distances" }
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/v1/collections/{collectionId}/query", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ChromaQueryResponse>();
        var results = new List<QueryResult>();

        if (result?.Documents is { Count: > 0 } && result.Documents[0] is { Count: > 0 })
        {
            for (int i = 0; i < result.Documents[0].Count; i++)
            {
                results.Add(new QueryResult
                {
                    Document = result.Documents[0][i] ?? "",
                    Metadata = result.Metadatas?[0]?[i] ?? new Dictionary<string, JsonElement>(),
                    Distance = result.Distances?[0]?[i] ?? 0
                });
            }
        }

        return results;
    }

    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/heartbeat");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class ChromaQueryResponse
{
    [JsonPropertyName("ids")]
    public List<List<string>>? Ids { get; set; }

    [JsonPropertyName("documents")]
    public List<List<string?>>? Documents { get; set; }

    [JsonPropertyName("metadatas")]
    public List<List<Dictionary<string, JsonElement>?>>? Metadatas { get; set; }

    [JsonPropertyName("distances")]
    public List<List<float>>? Distances { get; set; }
}

public class QueryResult
{
    public string Document { get; set; } = "";
    public Dictionary<string, JsonElement> Metadata { get; set; } = new();
    public float Distance { get; set; }
}
