```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
AMD Ryzen 5 5500U with Radeon Graphics 2.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.420
  [Host]     : .NET 8.0.26 (8.0.26, 8.0.2626.16921), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 8.0.26 (8.0.26, 8.0.2626.16921), X64 RyuJIT x86-64-v3


```
| Method       | Mean    | Error    | StdDev   | Gen0        | Gen1       | Gen2      | Allocated |
|------------- |--------:|---------:|---------:|------------:|-----------:|----------:|----------:|
| LoadAsList   | 3.036 s | 0.0598 s | 0.0877 s | 121000.0000 | 61000.0000 | 1000.0000 | 746.49 MB |
| LoadAsStream | 2.970 s | 0.0595 s | 0.0944 s | 120000.0000 | 60000.0000 | 1000.0000 | 744.38 MB |
