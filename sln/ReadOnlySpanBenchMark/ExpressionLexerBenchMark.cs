using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.UriParser;

namespace ReadOnlySpanBenchMark;

[MemoryDiagnoser]
public class ExpressionLexerBenchMark
{
    int NumberOfItems = 100000;
    public string expression = "abc(2014-09-19T12:13:14+00:00)/3258.678765765489753678965390/SRID=1234(POINT(10 20))/Function(foo=@x,bar=1,baz=@y)";

    public void Visit()
    {
        ExpressionLexer lexer = new Microsoft.OData.UriParser.ExpressionLexer(expression, true, false);
        while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        {
            Console.WriteLine($"Kind: {lexer.CurrentToken.Kind}: Text: {lexer.CurrentToken.Text}");

            lexer.NextToken();
        }
    }

    [Benchmark]
    public void ExpressionLexerUsingNewStringSingleRun()
    {
        ExpressionLexer2 lexer = new Microsoft.OData.UriParser.ExpressionLexer2(expression, true, false);
        while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        {
            lexer.NextToken();
        }
    }

    [Benchmark]
    public void ExpressionLexerUsingReadOnlySingleRun()
    {

        ExpressionLexer lexer = new Microsoft.OData.UriParser.ExpressionLexer(expression, true, false);
        while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
        {
            lexer.NextToken();
        }
    }

    [Benchmark]
    public void ExpressionLexerUsingNewString()
    {
        for (int i = 0; i < NumberOfItems; i++)
        {
            ExpressionLexer2 lexer = new Microsoft.OData.UriParser.ExpressionLexer2(expression, true, false);
            while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                lexer.NextToken();
            }
        }
    }

    [Benchmark]
    public void ExpressionLexerUsingReadOnly()
    {
        for (int i = 0; i < NumberOfItems; i++)
        {
            ExpressionLexer lexer = new Microsoft.OData.UriParser.ExpressionLexer(expression, true, false);
            while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                lexer.NextToken();
            }
        }
    }
}
