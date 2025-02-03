namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hasExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hasExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hasExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hasExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hasExpr)!, input);
}

var _ʺx68x61x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx68x61x73ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx68x61x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hasExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx68x61x73ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hasExpr)!, input);
}

var _enum_1 = __GeneratedOdataV3.Parsers.Rules._enumParser.Instance.Parse(_RWS_2.Remainder);
if (!_enum_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hasExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hasExpr(_RWS_1.Parsed, _ʺx68x61x73ʺ_1.Parsed, _RWS_2.Parsed,  _enum_1.Parsed), _enum_1.Remainder);
            }
        }
    }
    
}
