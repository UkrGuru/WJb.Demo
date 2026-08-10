# WJb Demos

This repository contains demonstration projects for WJb.

## Community Edition

### [WJb.Demo.MinApi](./WJb.Demo.MinApi)

A minimal ASP.NET Core API demonstrating:

- Job creation
- Background execution
- Progress reporting
- Result retrieval
- In-memory storage

### [WJb.Demo.Monitor](./WJb.Demo.Monitor)

A monitoring application demonstrating:

- Workflow monitoring
- Real-time progress visualization
- Job payload inspection
- Job result inspection
- Failure diagnostics
- Retry support
- Action discovery
- Service discovery
- Definition management

This sample demonstrates a complete monitoring dashboard for WJb workflows and background job execution.

### [WJb.Demo.Wasm](./WJb.Demo.Wasm)

A Blazor WebAssembly application demonstrating:

- Client-side job execution
- Progress reporting
- Action chaining
- Retry workflows
- Multi-step workflows
- Result inspection
- Browser-based processing
- Zero server-side workers
- Built-in source viewer

Included scenarios:

- Hello Action
- Configured Action
- Progress Action
- Chained Actions
- Retry Workflow
- Order Workflow

This sample demonstrates running WJb entirely in the browser without background services or server-side job execution.

## Commercial Edition

### WJb.Sql.Demo

Demonstrates SQL Server integration using WJb.Sql.

Features include:

- Persistent job storage
- Durable execution
- Multi-instance processing
- Production-oriented configuration

> Available only in the commercial edition.

### WJb.Pro.Demo

Demonstrates advanced capabilities available in WJb.Pro.

> Available only in the commercial edition.

## Getting Started

Recommended learning path:

1. **WJb.Demo.MinApi** - Core concepts
2. **WJb.Demo.Wasm** - Actions, workflows, retries, and execution
3. **WJb.Demo.Monitor** - Monitoring, diagnostics, and administration

Start with the Community Edition demos to learn the core WJb concepts and workflow.
