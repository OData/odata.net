namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_entityIdOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption)!, input);
}

var _entityIdOption_1 = __GeneratedOdataV3.Parsers.Rules._entityIdOptionParser.Instance.Parse(_ʺx26ʺ_1.Remainder);
if (!_entityIdOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityIdOption(_ʺx26ʺ_1.Parsed, _entityIdOption_1.Parsed), _entityIdOption_1.Remainder);
            }
        }
    }
    
}
