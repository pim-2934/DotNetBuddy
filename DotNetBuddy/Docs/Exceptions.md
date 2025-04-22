# Exception Handling

## Purpose

Provides centralized HTTP exception handling through `Buddy.NET`.

## Usage

Enable exception handling middleware:

```csharp
app.UseBuddyExceptions();
```

## Features

- Handles and serializes exceptions using RFC 9110 compliance.
- Outputs JSON error responses in production environments.
- Logs to console in development.
