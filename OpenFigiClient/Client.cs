using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
namespace OpenFigiClient;

public class Client(string? figiKey = null)
{
    private const string FigiKey = "X-OPENFIGI-APIKEY";
    private readonly string FigiKeyValue = figiKey ??
            Environment.GetEnvironmentVariable(FigiKey) ??
            throw new UnauthorizedAccessException($"Environment variable not found: {FigiKey}.");

    public async Task<JsonArray> GetMappingAsync(string idType, string idValue, CancellationToken ct)
    {
        Uri uri = new("https://api.openfigi.com/v3/mapping");
        string request = $"[{{\"idType\":\"{idType}\",\"idValue\":\"{idValue}\"}}]";

        string result = await GetAsync(uri, request, ct).ConfigureAwait(false);

        JsonNode root = JsonNode.Parse(result) ?? throw new ArgumentException("Could not parse result.");
        JsonArray array = (root as JsonArray) ?? throw new ArgumentException("Expected a JSON array.");
        JsonNode item = array.Single() ?? throw new ArgumentException("No item found.");
        JsonNode data = item["data"] ?? throw new ArgumentException("No data item found.");
        JsonArray ja = data as JsonArray ?? throw new ArgumentException("Expected a JSON array.");

        return ja;
    }

    public async Task<string> GetAsync(Uri uri, string request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request))
            throw new ArgumentException("Invalid request.");

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add(FigiKey, FigiKeyValue);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        StringContent content = new(request, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(uri, content, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        string str = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return str;
    }

    public static string PrintAllValues(JsonNode node)
    {
        StringBuilder sb = new();

        PrintAllValues(node, "");

        return sb.ToString();

        void PrintAllValues(JsonNode? node, string path)
        {
            if (node == null)
                return;

            switch (node)
            {
                case JsonValue value:
                    // Try to get as string for display
                    string display = value.ToJsonString();
                    sb.AppendLine($"{path} = {display}");
                    break;

                case JsonObject obj:
                    foreach (var kvp in obj)
                    {
                        string childPath = string.IsNullOrEmpty(path) ? kvp.Key : $"{path}.{kvp.Key}";
                        PrintAllValues(kvp.Value, childPath);
                    }
                    break;

                case JsonArray arr:
                    for (int i = 0; i < arr.Count; i++)
                    {
                        string childPath = $"{path}[{i}]";
                        PrintAllValues(arr[i], childPath);
                    }
                    break;
            }
        }
    }
}