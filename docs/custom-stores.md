# Custom Stores

WJb is storage-agnostic.

The core package does not require SQL Server, Redis, MongoDB, or any specific database.

```text
       WJb
        ↓
     IStore
        ↓
  Your Storage
```

You decide how jobs are stored.

---

## Why Custom Stores?

Different applications have different requirements.

Examples:

```text
SQL Server
PostgreSQL
SQLite
Redis
MongoDB
Cloud Storage
IndexedDB
```

WJb allows each application to choose the most appropriate storage solution.

---

## Store Responsibilities

A store is responsible for:

✅ Enqueueing jobs  
✅ Dequeueing jobs  
✅ Updating progress  
✅ Completing jobs  
✅ Recording failures  
✅ Querying jobs  
✅ Deleting jobs  

A store is **persistence**.

Nothing more.

---

## Minimal Architecture

```text
Application
      ↓
   Executor
      ↓
    IStore
      ↓
  Database / Storage
```

The executor runs actions.  
The store persists data.

---

## Key Interfaces (v1.1)

From the official API tree:

```text
IStore
  └── StoreBase
        ├── InMemoryStore
        └── DbStoreBase

IDefinitionStore
ICronStore
IJobStoreQuery
IDbStore
```

Most custom stores implement `IStore` (or inherit from `StoreBase` / `DbStoreBase`).

---

## Job Identifiers

Job identifiers use strings.

```csharp
public string Id { get; init; }
```

This allows each store to choose its own internal key format.

Examples:

```text
Guid
long
ULID
ObjectId
Snowflake
```

The public contract remains the same.

---

## Payloads

Payloads are stored as JSON.

```csharp
JsonNode
```

Examples:

```json
{
  "email": "user@test.com"
}
```

```json
{
  "customerId": 42
}
```

```json
[1, 2, 3]
```

Any valid JSON can be stored.

---

## Results

Results are also stored as JSON.

Examples:

```json
123
```

```json
"Done"
```

```json
true
```

```json
{
  "sent": true
}
```

Stores should support any valid JSON value.

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

The executor does not require a specific error format.

---

## Queue Support

Stores should preserve queue information.

Example:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Queue = "email"
    });
```

Workers can then process only that queue.

---

## Scheduled Jobs

Stores must support delayed execution.

Example:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromMinutes(5)
    });
```

The job should not be dequeued before its scheduled time.

---

## Defensive Copies

Stores should avoid exposing internal mutable state.

When a job is stored:

```text
Caller
   ↓
 Store
```

Changes made by the caller afterwards must **not** modify the stored job.

---

## InMemoryStore

The built-in `InMemoryStore` provides a reference implementation.

Useful for:

- Development
- Testing
- Learning
- Custom provider implementations

Reading its implementation is the recommended starting point for building a new provider.

```csharp
var store = new InMemoryStore();
var wjb = WJbBuilder.Create(store, cfg => { ... });
```

---

## Recommended Base Classes

For most custom stores:

- Inherit from `StoreBase` (general purpose)
- Or inherit from `DbStoreBase` (when working with relational databases)

These base classes already implement common boilerplate.

---

## Best Practices

✅ Keep storage concerns inside the store  
✅ Preserve JSON values exactly  
✅ Support scheduled execution  
✅ Support queues  
✅ Keep implementations simple  
✅ Use the identifier type best suited to your database  
✅ Prefer inheriting `StoreBase` / `DbStoreBase`  

❌ Put business logic in the store  
❌ Depend on specific action types  
❌ Modify payloads during persistence  
❌ Assume identifiers are GUIDs  

---

## Mental Model

```text
Action   = Business Logic
Executor = Execution
Store    = Persistence
```

A store should answer one question:

> How are jobs persisted?

Nothing else should influence its design.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/11_CustomStoresTests.cs
```
