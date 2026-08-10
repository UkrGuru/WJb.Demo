# 🚀 New WJb Demo: Workflow Monitoring in Action

Want to see what a complete workflow looks like in WJb?

The new **WJb.Demo.Monitor** sample demonstrates an end-to-end workflow with live monitoring, action management, service configuration, and job inspection.

![WJb Monitor](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-monitor.png)

✅ Import Customers  
✅ Generate Report  
✅ Send Email  
✅ Monitor execution in real time

Run a workflow:

```csharp
await wjb.EnqueueAsync(
    ImportCustomersAction.Key,
    new ImportCustomersInput
    {
        Source = "CRM"
    });
```

WJb automatically executes the workflow:

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
- Real-time monitoring
- Job failure handling
- Payload inspection
- Action discovery
- Service discovery

## Monitor Features

### Jobs

The Jobs page provides:

- Status tracking
- Progress visualization
- Retry support
- Failure diagnostics
- Payload inspection
- Execution history
- Date filtering
- Paging

### Actions

The Actions page provides:

- Registered action discovery
- Action metadata inspection
- Action definition editing
- Action testing
- JSON configuration editing

### Services

The Services page provides:

- Registered service discovery
- Service configuration editing
- JSON configuration inspection

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
- Retry handling
- Progress reporting
- Live monitoring
- Payload inspection
- Failure inspection
- Actions explorer
- Services explorer
- Configuration editing

Perfect for understanding how WJb workflows behave in a real application and how they can be monitored in production.
