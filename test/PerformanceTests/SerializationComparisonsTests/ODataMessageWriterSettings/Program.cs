// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run(typeof(Program).Assembly);
