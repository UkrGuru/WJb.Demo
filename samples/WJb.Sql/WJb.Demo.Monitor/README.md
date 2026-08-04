# 🚀 New WJb Demo: Workflow Monitoring in Action

Want to see what a complete workflow looks like in WJb?

The new **WJb.Demo.Monitor** sample demonstrates an end-to-end job pipeline with a live monitoring UI.

![WJb Monitor](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-monitor.png)

✅ Import Customers  
✅ Generate Report  
✅ Send Email  
✅ Monitor execution in real time

Run a single action:

```csharp
await wjb.EnqueueAsync(
    ImportCustomersAction.Key,
    new ImportCustomersInput
    {
        Source = "CRM"
    });
```

WJb automatically chains the workflow:

```text
import-customers
        ↓
generate-report
        ↓
send-email
```

Every step becomes visible in the monitoring dashboard.

## What the sample demonstrates

- Typed action inputs
- Constructor dependency injection
- Workflow chaining via `IAction.NextAsync(...)`
- Progress reporting
- Background job execution
- Real-time monitoring UI
- Job payload inspection
- Action discovery
- Service discovery

## Monitor Features

The built-in Blazor monitor provides:

- Job status tracking
- Progress visualization
- Execution history
- Payload inspection
- Registered actions
- Registered services

## Why it is interesting

The entire workflow is composed of small focused actions:

- `import-customers`
- `generate-report`
- `send-email`

No workflow designer.

No XML.

No complex configuration.

Just C# actions connected through explicit workflow transitions.

## Sample

🔗 https://github.com/UkrGuru/WJb.Demo/tree/main/samples/WJb.Demo.Monitor

The sample includes:

- Workflow execution
- Action chaining
- Progress reporting
- Live monitoring
- Payload inspection
- Actions explorer
- Services explorer

Perfect for understanding how WJb workflows behave in a real application.
