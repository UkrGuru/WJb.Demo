# Store

A store is responsible for persisting jobs.

WJb does not require a specific database or storage technology.

```text
Executor
    ↓
 Store
    ↓
Persistence
```

The executor runs jobs.

The store decides how jobs are stored.

---

## Built-in Stores

### InMemoryStore

Stores jobs in memory.

```csharp
var store = new InMemoryStore();
```

Useful for:

- Development
- Testing
- Prototyping

Jobs are lost when the application stops.

---

### WJb.Sql

Commercial SQL Server storage provider.

```csharp
var store =
    new SqlStore(connectionFactory);
```

Useful for:

- Production workloads
- Multiple processes
- Persistent job history

---

## Using a Store

```csharp
var store = new InMemoryStore();

var wjb = WJbBuilder.Create(store);
```

Every job operation goes through the store:

```text
Enqueue
Dequeue
Complete
Fail
Update Progress
Query Jobs
Delete Jobs
```

---

## Custom Stores

You can implement your own storage provider.

Examples:

- PostgreSQL
- MySQL
- SQLite
- Redis
- MongoDB
- Cloud Services

---

## Job Identifiers

Job identifiers are stored as strings.

```csharp
public string Id { get; init; }
```

This allows a store to use any internal identifier type.

Examples:

```text
Guid
```

```text
long
```

```text
ULID
```

```text
ObjectId
```

```text
Snowflake
```

The storage implementation decides how identifiers are generated and stored.

---

## Payloads

Job payloads are stored as JSON.

```csharp
JsonNode
```

Payload examples:

```json
{
  "to": "user@test.com"
}
```

```json
{
  "customerId": 42
}
```

```json
{
  "file": "report.pdf"
}
```

The executor converts payloads into action input models automatically.

---

## Results

Job results are stored as JSON.

Examples:

```json
123
```

```json
"Done"
```

```json
{
  "sent": true
}
```

Result values are not limited to JSON objects.

---

## Errors

Failed jobs store error information.

Examples:

```json
"SMTP unavailable"
```

```json
{
  "message": "SMTP unavailable"
}
```

The error format is determined by the action and store.

---

## Large Content

Stores are designed for jobs, not files.

Preferred:

```text
Store
    ↓
Job Metadata
```

```text
Storage
    ↓
Large Files
```

Example:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    new
    {
        BodyId = htmlId
    });
```

Instead of:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    new
    {
        Body = hugeHtmlDocument
    });
```

---

## Best Practices

✅ Keep jobs small

✅ Keep payloads simple

✅ Store large content separately

✅ Choose storage based on workload

✅ Use custom stores when needed

❌ Store large files inside jobs

❌ Couple business logic to storage

❌ Assume a specific identifier type

---

## Mental Model

```text
Action   = Business Logic

Executor = Runs Actions

Store    = Saves Jobs
```

The store is persistence.

Nothing more.

Nothing less.