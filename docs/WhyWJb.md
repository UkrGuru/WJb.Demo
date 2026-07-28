# Why WJb?



WJb is a lightweight workflow engine for .NET that lets you orchestrate business processes using plain C# code.



No DSLs. No XML. No visual designers. No mandatory infrastructure.



Build workflows the same way you build the rest of your application.



\---



\# Key Advantages



\## 1. Zero Required Dependencies



WJb can be added to almost any .NET application without introducing infrastructure requirements.



You do \*\*not\*\* need:



\- SQL Server

\- PostgreSQL

\- Redis

\- RabbitMQ

\- Kafka

\- Docker

\- Kubernetes



Install the package and start building workflows immediately.



\---



\## 2. Works Anywhere .NET Runs



WJb is not tied to a specific hosting model.



Use it in:



\- ASP.NET Core applications

\- Console applications

\- Worker Services

\- Windows Services

\- Linux services

\- Docker containers

\- Azure-hosted applications

\- IIS-hosted applications



\---



\## 3. Fast and Simple to Start



Your first workflow can be running within minutes.



There is no need to:



\- Deploy additional services

\- Configure message brokers

\- Learn a workflow-specific language

\- Set up distributed infrastructure



\---



\## 4. Workflows Are Just C#



WJb embraces the language you already use every day.



No:



\- DSL

\- XML

\- YAML

\- JSON workflow definitions

\- Visual process designers



Use familiar .NET concepts:



\- Classes

\- Interfaces

\- Dependency Injection

\- Generics

\- Async/await



\---



\## 5. Full IDE Support



Because workflows are regular C# code, your IDE understands them completely.



You get:



\- Go To Definition

\- Find Usages

\- Rename Refactoring

\- Code Navigation

\- IntelliSense

\- Debugging

\- Breakpoints



No special tooling required.



\---



\## 6. Minimal Learning Curve



If you already know:



\- C#

\- Dependency Injection

\- Async programming



you already know most of what is needed to be productive with WJb.



\---



\## 7. Actions Are Simple Classes



Each workflow step is an individual Action.



Benefits:



\- Clear responsibility

\- Easy maintenance

\- Reusability

\- Better testability

\- Better readability



\---



\## 8. Readable Business Processes



Large business processes often become scattered across services, controllers, event handlers, and background tasks.



WJb keeps workflow logic organized and visible.



A developer can quickly understand:



\- Where execution starts

\- Which steps run

\- Where decisions are made

\- How the process ends



\---



\## 9. Reusable Building Blocks



Actions can be reused across multiple workflows.



For example:



\- Validation actions

\- API integration actions

\- Email actions

\- Database actions



Build once and use everywhere.



\---



\## 10. Easy Unit Testing



Workflows and Actions can be tested independently.



You can test:



\- Single actions

\- Action chains

\- Complete workflows



Without running external infrastructure.



\---



\## 11. Architecture-Friendly



WJb does not force a specific architectural style.



It works well with:



\- Clean Architecture

\- Modular Monolith

\- Vertical Slice Architecture

\- Layered Architecture

\- CQRS



WJb complements your architecture instead of replacing it.



\---



\## 12. No Mandatory Message Queues



Many workflow systems assume queues are always required.



WJb does not.



If your solution needs a queue, use one.



If it does not, you can keep your system simple.



\---



\## 13. No Mandatory Database



For simple scenarios, workflows can run entirely in memory.



When persistent state becomes necessary, you can add \*\*WJb.Sql\*\* without changing your workflow design.



\---



\## 14. Grow at Your Own Pace



A typical adoption path looks like this:



\### Step 1



Use WJb for workflow orchestration.



\### Step 2



Add WJb.Sql for persistence and durability.



\### Step 3



Add WJb.Pro for monitoring and management capabilities.



Start small and expand only when needed.



\---



\## 15. Complete Workflow Visibility



A workflow is defined in one place.



Developers can easily see:



\- Entry points

\- Workflow steps

\- Branches

\- Error paths

\- Completion paths



This significantly improves maintainability.



\---



\## 16. Ideal for Long-Running Processes



WJb is a natural fit for workflows such as:



\- User onboarding

\- Order processing

\- Approval flows

\- Document processing

\- Data imports

\- System integrations



\---



\## 17. Small Core, Few Concepts



You do not need to learn an entirely new platform.



The core concepts are intentionally small and focused.



This helps teams become productive quickly.



\---



\## 18. Perfect for Small Teams



Many workflow platforms assume large teams and dedicated infrastructure engineers.



WJb works especially well for:



\- Solo developers

\- Small development teams

\- Startups

\- Internal business applications



\---



\## 19. Ready for Enterprise Systems



WJb can support workflows behind:



\- CRM systems

\- ERP systems

\- Financial platforms

\- Document management systems

\- Integration solutions



Simple enough to start, powerful enough to scale.



\---



\## 20. Stays Inside Your Application



Your workflow engine remains part of your application rather than becoming a separate platform.



Benefits include:



\- One repository

\- One deployment pipeline

\- One testing strategy

\- One development experience



Everything remains in familiar .NET code.



\---



\# In One Sentence



\*\*WJb lets you build workflows using plain C#, with zero required infrastructure, full IDE support, excellent testability, and a clear upgrade path from simple in-memory workflows to production-grade persistent workflow orchestration.\*\*

