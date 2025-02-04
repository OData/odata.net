namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _entityIdOption_ʺx26ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ> Parse(IInput<char>? input)
            {
                var _entityIdOption_1 = __GeneratedOdataV3.Parsers.Rules._entityIdOptionParser.Instance.Parse(input);
if (!_entityIdOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ)!, input);
}

var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(_entityIdOption_1.Remainder);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._entityIdOption_ʺx26ʺ(_entityIdOption_1.Parsed, _ʺx26ʺ_1.Parsed), _ʺx26ʺ_1.Remainder);
            }
        }
    }
    
}
