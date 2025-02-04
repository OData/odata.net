namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤentityCastOption_ʺx26ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤentityCastOption_ʺx26ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤentityCastOption_ʺx26ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤentityCastOption_ʺx26ʺↃ> Parse(IInput<char>? input)
            {
                var _entityCastOption_ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._entityCastOption_ʺx26ʺParser.Instance.Parse(input);
if (!_entityCastOption_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤentityCastOption_ʺx26ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤentityCastOption_ʺx26ʺↃ(_entityCastOption_ʺx26ʺ_1.Parsed), _entityCastOption_ʺx26ʺ_1.Remainder);
            }
        }
    }
    
}
