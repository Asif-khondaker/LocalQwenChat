using System.Net.Http.Json;
using System.Text.Json;

namespace LocalQwenChat.Services;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<string> StreamChatAsync(
        string message,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = "qwen3:8b",

            messages = new[]
            {
                new
                {
                    role = "user",
                    content = message
                }
            },

            stream = true,

            // Disable Qwen thinking
            think = false,

            // Keep model loaded
            keep_alive = "30m"
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/chat")
        {
            Content = JsonContent.Create(requestBody)
        };

        Console.WriteLine(
            ">>> Sending request to Ollama...");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        Console.WriteLine(
            $"<<< Ollama response: {response.StatusCode}");

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken);

        using var reader = new StreamReader(stream);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(
                cancellationToken);

            if (line == null)
            {
                Console.WriteLine(
                    "<<< Ollama stream ended.");

                break;
            }

            if (string.IsNullOrWhiteSpace(line))
                continue;

            Console.WriteLine(
                $"OLLAMA RAW: {line}");

            OllamaStreamResponse? chunk;

            try
            {
                chunk =
                    JsonSerializer.Deserialize<OllamaStreamResponse>(
                        line,
                        JsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine(
                    $"JSON ERROR: {ex.Message}");

                continue;
            }

            if (chunk?.Message?.Content is { Length: > 0 } content)
            {
                Console.WriteLine(
                    $"CONTENT: [{content}]");

                yield return content;
            }

            if (chunk?.Done == true)
            {
                Console.WriteLine(
                    "<<< Ollama reported DONE.");

                break;
            }
        }
    }

    private class OllamaStreamResponse
    {
        public OllamaMessage? Message { get; set; }

        public bool Done { get; set; }
    }

    private class OllamaMessage
    {
        public string Content { get; set; } = string.Empty;
    }
}