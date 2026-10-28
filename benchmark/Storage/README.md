# Persistent Storage Benchmark

Comparison of WJb persistent storage providers.

## Scenario

1. Enqueue N jobs
2. Dequeue all jobs
3. Verify all jobs were processed

## Results

### 100 Jobs

| Provider | Mean |
|----------|------:|
| WJb.SqlServer | 85 ms |
| WJb.SQLite | 1420 ms |

WJb.SqlServer is approximately **16x faster**.

## Conclusion

🥇 **WJb.SqlServer** delivers maximum throughput for persistent storage.

🥈 **WJb.SQLite** provides a lightweight embedded option with no server dependency.

Choose:

- **WJb.SQLite** for local, desktop and single-user scenarios.
- **WJb.SqlServer** for server, cloud and high-throughput workloads.