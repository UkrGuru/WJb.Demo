# WJb.DocTests

Documentation verification tests for WJb.

This project ensures that examples from the WJb documentation remain valid as the API evolves.

```text
Documentation
       ↓
   DocTests
       ↓
Continuous Validation
```

The goal is simple:

> Every documented concept should have a corresponding test.

---

## Why?

Documentation often becomes outdated.

Examples that looked correct when written may stop compiling or no longer reflect actual behavior.

`WJb.DocTests` helps prevent that.

Instead of trusting examples, we verify them.

---

## What Is Tested?

Each documentation page has a matching test class.

```text
actions.md
    ↓
01 ActionsTests.cs

action-result.md
    ↓
02 ActionResultTests.cs

job-command.md
    ↓
03 JobCommandTests.cs

job-options.md
    ↓
04 JobOptionsTests.cs

executor.md
    ↓
05 ExecutorTests.cs

progress.md
    ↓
06 ProgressTests.cs

retry.md
    ↓
07 RetryTests.cs

scheduling.md
    ↓
08 SchedulingTests.cs

queues.md
    ↓
09 QueuesTests.cs

store.md
    ↓
10 StoreTests.cs

custom-stores.md
    ↓
11 CustomStoresTests.cs

wjb-sql.md
    ↓
12 WjbSqlTests.cs

faq.md
    ↓
13 FaqTests.cs
```

---

## What These Tests Are Not

These tests are **not** intended to replace:

- Unit tests
- Integration tests
- End-to-end tests

Those belong in other projects.

`WJb.DocTests` focuses only on documented behavior and sample code.

---

## Typical Examples

Examples covered by documentation tests include:

```csharp
ActionResults.None()
```

```csharp
ActionResults.Result(...)
```

```csharp
ActionResults.Next(...)
```

```csharp
new JobCommand(...)
```

```csharp
new JobOptions
{
    Delay = ...
}
```

```csharp
new JobOptions
{
    MaxRetries = ...
}
```

---

## Benefits

✅ Documentation stays accurate

✅ Public API changes are detected quickly

✅ Samples remain executable

✅ Examples remain aligned with the implementation

✅ Contributors can update documentation with confidence

---

## Example

A documentation page may contain:

```csharp
var result =
    ActionResults.Result(123);
```

The corresponding test verifies that the example continues to work.

```csharp
[Fact]
public void ActionResults_Should_Support_Integer_Result()
{
    var result = ActionResults.Result(123);

    Assert.Equal(123, result.Value);
}
```

---

## Workflow

```text
Change API
     ↓
Run Tests
     ↓
DocTests Fail
     ↓
Update Docs
     ↓
Docs Match Code
```

This helps ensure that documentation and implementation evolve together.

---

## Current Coverage

```text
13 Documentation Pages

82 Documentation Tests
```

Every test must pass.

A failing documentation test indicates that a documented example or documented behavior no longer matches the implementation.

---

## Mental Model

```text
Unit Tests
      ↓
Verify Components

Integration Tests
      ↓
Verify Workflows

Documentation Tests
      ↓
Verify Examples
```

`WJb.DocTests` exists to prove that the documentation is correct.