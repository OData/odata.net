namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions)!, input);
}

var _queryOptions_1 = __GeneratedOdataV3.Parsers.Rules._queryOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_queryOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1.Parsed,  _queryOptions_1.Parsed), _queryOptions_1.Remainder);
            }
        }
    }
    
}
