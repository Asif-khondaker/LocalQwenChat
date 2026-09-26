namespace LocalQwenChat.Services;

public interface IOllamaService
{
    IAsyncEnumerable<string> StreamChatAsync(
        string message,
        CancellationToken cancellationToken = default);
}