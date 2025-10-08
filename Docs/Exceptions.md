# Exception Handling

## Purpose

Provides centralized HTTP exception handling through `DotNetBuddy`.

## Setup

Enable exception handling middleware:

```csharp
app.UseBuddyExceptions();
```

## Usage

```csharp
public class AccountInactiveException(string message)
    : ResponseException
    (
        message,
        StatusCodes.Status401Unauthorized
    );
```

## Example

```csharp
throw new AccountInactiveException("Please contact your administrator.");
```
Results in valid RFC 9110 compliant response:
```json
{
  "title": "AccountInactive",
  "status": 401,
  "detail": "Please contact your administrator.",
  "instance": "GET /endpoint",
  "requestId": "0HND46TRTPUVL:0000000F",
  "traceId": "00-4eea791dd06b2f09700e8e8d337b7747-8d69d0608ae047b3-00"
}
```

## Features

- Handles and serializes exceptions using RFC 7807 compliance.
- Outputs JSON error responses.
