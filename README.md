\# LocalQwenChat



A local AI chat application built with ASP.NET Core Razor Pages and Ollama.



\## Technology



\- ASP.NET Core

\- Razor Pages

\- C#

\- Ollama

\- Qwen3 8B

\- HTTP Streaming

\- JavaScript Fetch API



\## Architecture





Browser

&#x20;  |

&#x20;  | POST /api/chat

&#x20;  v

ASP.NET Core

&#x20;  |

&#x20;  v

OllamaService

&#x20;  |

&#x20;  | HTTP

&#x20;  v

Ollama

&#x20;  |

&#x20;  v

Qwen3 8B

