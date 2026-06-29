# 📦 WJb.Demo (WASM)

Lightweight demo application showcasing **WJb background job engine** running in a **Blazor WebAssembly (WASM)** environment.

![WJb.Demo (WASM) ](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-demo-wasm.png)

***

## 🚀 Overview

**WJb.Demo (WASM)** demonstrates how to:

* Execute background jobs in a client-side environment
* Track job lifecycle and progress in real time
* Use clean, decoupled job actions
* Chain and configure actions
* Handle success, failure, and cancellation scenarios

***

## 🧩 Features

* ✅ Job dashboard with live updates
* ✅ Multiple action types:
  * Hello Action
  * Configured Action
  * Progress Action
  * Chained Actions
* ✅ Progress tracking (0–100%)
* ✅ Status handling:
  * Completed
  * Failed
* ✅ Message output per job
* ✅ Job cleanup support

***

## 🖥️ UI Overview

The demo UI includes:

* **Sidebar navigation**
  * Dashboard
  * Action samples
* **Active Jobs table**
  * ID
  * Status
  * Progress bar
  * Message
  * Remove action

Example states:

* Completed jobs with full progress
* Failed jobs with partial progress
* Informational messages for each job

***

## ⚙️ How It Works

WJb separates concerns into:

* **Execution** – runs actions
* **Storage** – tracks job state
* **Hosting** – coordinates lifecycle

In this demo:

* Jobs are triggered from UI
* Execution happens in WASM runtime
* Progress updates are reflected immediately

***

## ▶️ Run Locally

```bash
git clone https://github.com/UkrGuru/WJb.Demo
cd WJb.Demo
dotnet run
```

Then open:

```
https://localhost:7268
```

***

## 🧪 Example Scenarios

* Run simple job (Hello Action)
* Execute configured job (SMTP simulation)
* Track progress over time
* Trigger chained jobs
* Observe failure handling

***

## 📌 Notes

* This is a **demo application** intended for showcasing WJb capabilities.
* Behavior and APIs may evolve.
* Some features may be simplified for demonstration purposes.

***
