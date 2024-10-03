// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Microsoft.OData.UriParser;
using ReadOnlySpanBenchMark;


//new ExpressionLexerBenchMark().Visit();

BenchmarkRunner.Run<ExpressionLexerBenchMark>();
