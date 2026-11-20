# SyncDemo

`SyncDemo` demonstrates how a long-running synchronization process can be resumed after an unexpected shutdown.

This sample intentionally avoids SQL providers and uses a simple file-based checkpoint mechanism to make the workflow easy to understand.

## What this demo shows

The sample consists of two parts:

### SyncSdk

Responsible for business logic:

- Build synchronization plan
- Detect Creates
- Detect Updates
- Detect Deletes

### WJb

Responsible for execution:

- Run Actions
- Execute long-running jobs
- Resume after restart
- Continue from the last checkpoint

---

## How it works

Source data:

```text
App_Data/Source/customers.json
```

Target data:

```text
App_Data/Target/customers.json
```

Checkpoint state:

```text
App_Data/State/checkpoint.json
```

The synchronization runs in three phases:

```text
DELETE
    ↓
UPDATE
    ↓
CREATE
```

After every processed item the current state is saved:

```json
{
  "phase": 1,
  "index": 3
}
```

If the application stops, the next run loads the checkpoint and continues from the saved position.

---

## First Run

Before running the sample for the first time:

```text
Delete:
- bin
- obj
```

This guarantees a completely clean start.

Also delete:

```text
App_Data/State/checkpoint.json
```

if it exists.

---

## Running the Demo

Start the application:

```bash
dotnet run
```

Example output:

```text
Starting Sync Demo...

Sync job queued.

DELETE CUS-010

DELETE phase completed

UPDATE CUS-001

CREATE CUS-002
CREATE CUS-003

SYNC COMPLETED
```

---

## Testing Recovery

Start the application.

While synchronization is running:

```text
Close the console window
```

or

```text
Stop debugging
```

at any moment.

For example:

```text
Starting Sync Demo...

Sync job queued.

DELETE CUS-010

DELETE phase completed

UPDATE CUS-001
```

Stop the process.

A checkpoint will already be stored:

```json
{
  "phase": 1,
  "index": 1
}
```

Run the application again:

```bash
dotnet run
```

Synchronization continues from the saved position.

Example:

```text
Starting Sync Demo...

Sync job queued.

UPDATE phase completed

CREATE CUS-002
CREATE CUS-003

CREATE phase completed

SYNC COMPLETED
```

---

## Verifying Results

After synchronization completes, inspect:

```text
App_Data/Target/customers.json
```

You should see that:

- deleted records were removed
- updated records were updated
- new records were created

without starting from the beginning.

---

## Why File-Based Checkpoints?

This sample intentionally uses:

```text
checkpoint.json
```

to keep the implementation simple and transparent.

You can open the file and immediately see:

```json
{
  "phase": "Update",
  "index": 1
}
```

which makes it ideal for learning and experimentation.

---

## Commercial Version

⚠️ This sample demonstrates a non-commercial approach.

The file-based checkpoint mechanism exists only to illustrate the concept.

For production workloads this approach has limitations:

- file corruption
- process crashes during writes
- concurrency concerns
- deployment complexity
- recovery edge cases

The commercial version will provide a much simpler and more reliable solution.

A future sample will be added alongside this one:

```text
SyncDemoSql
```

using the commercial WJb SQL-based persistence implementation.

The goal of `SyncDemo` is educational.

The goal of `SyncDemoSql` will be production-grade reliability.

---

## Summary

This demo illustrates:

✅ Sync planning  
✅ Create / Update / Delete phases  
✅ Checkpoint persistence  
✅ Recovery after process restart  
✅ Long-running Action execution  
✅ WJb-driven synchronization workflow

without requiring any database infrastructure.