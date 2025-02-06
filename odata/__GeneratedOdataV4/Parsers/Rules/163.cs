namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _maxDateTimeMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺParser.Instance.Parse(input);
if (!_ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(_ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV4.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._maxDateTimeMethodCallExpr(_ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
