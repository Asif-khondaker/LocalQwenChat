using LocalQwenChat.Models;
using LocalQwenChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IOllamaService, OllamaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = Timeout.InfiniteTimeSpan;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


// ===============================
// Ollama Streaming API
// ===============================
app.MapPost("/api/chat", async (
    ChatRequest request,
    IOllamaService ollamaService,
    HttpContext httpContext) =>
{
    Console.WriteLine(">>> /api/chat CALLED");
    Console.WriteLine($"Message: {request.Message}");

    if (string.IsNullOrWhiteSpace(request.Message))
    {
        httpContext.Response.StatusCode = 400;

        await httpContext.Response.WriteAsync(
            "Message is required.",
            httpContext.RequestAborted);

        return;
    }

    httpContext.Response.StatusCode = 200;

    httpContext.Response.ContentType =
        "text/plain; charset=utf-8";

    httpContext.Response.Headers.CacheControl =
        "no-cache, no-store";

    httpContext.Response.Headers["X-Accel-Buffering"] =
        "no";

    try
    {
        await foreach (var chunk in ollamaService.StreamChatAsync(
            request.Message,
            httpContext.RequestAborted))
        {
            Console.WriteLine($"Chunk: [{chunk}]");

            await httpContext.Response.WriteAsync(
                chunk,
                httpContext.RequestAborted);

            await httpContext.Response.Body.FlushAsync(
                httpContext.RequestAborted);
        }

        Console.WriteLine(">>> /api/chat FINISHED");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine(">>> Request cancelled");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"!!! ERROR: {ex}");

        if (!httpContext.Response.HasStarted)
        {
            httpContext.Response.StatusCode = 500;

            await httpContext.Response.WriteAsync(
                "Server error: " + ex.Message);
        }
    }
});

app.Run();