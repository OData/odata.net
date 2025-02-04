namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_entityCastOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption)!, input);
}

var _entityCastOption_1 = __GeneratedOdataV3.Parsers.Rules._entityCastOptionParser.Instance.Parse(_ʺx26ʺ_1.Remainder);
if (!_entityCastOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_entityCastOption(_ʺx26ʺ_1.Parsed, _entityCastOption_1.Parsed), _entityCastOption_1.Remainder);
            }
        }
    }
    
}
