```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 5500U with Radeon Graphics 2.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.419
  [Host]     : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3


```
| Method       | Mean     | Error   | StdDev   | Gen0       | Gen1      | Gen2      | Allocated |
|------------- |---------:|--------:|---------:|-----------:|----------:|----------:|----------:|
| LoadAsList   | 317.6 ms | 6.29 ms | 12.42 ms | 13000.0000 | 7000.0000 | 1000.0000 |  76.81 MB |
| LoadAsStream | 300.8 ms | 5.41 ms |  4.79 ms | 13000.0000 | 7000.0000 | 1000.0000 |  76.44 MB |
