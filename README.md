# LocalQwenChat

A simple local AI chat application built with **ASP.NET Core Razor Pages + Ollama + Qwen3 8B**.

## How It Works

```text
Browser
   ↓
ASP.NET Core /api/chat
   ↓
OllamaService
   ↓
Ollama
   ↓
Qwen3 8B
   ↓
Streaming response → Browser
```

The browser sends the user's message to `/api/chat`. `OllamaService` sends it to the local Ollama server, reads Qwen's streaming response, and ASP.NET streams the generated text back to the browser.

## Setup & Run

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and [Ollama](https://ollama.com/download/windows).
2. Download Qwen3 8B with `ollama pull qwen3:8b`, then clone this repository and run `dotnet run`; open `http://localhost:5160`.

## Repository

[GitHub – LocalQwenChat](https://github.com/Asif-khondaker/LocalQwenChat)
