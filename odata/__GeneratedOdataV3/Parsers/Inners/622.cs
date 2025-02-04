namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query)!, input);
}

var _query_1 = __GeneratedOdataV3.Parsers.Rules._queryParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_query_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_query(_ʺx3Fʺ_1.Parsed, _query_1.Parsed), _query_1.Remainder);
            }
        }
    }
    
}
