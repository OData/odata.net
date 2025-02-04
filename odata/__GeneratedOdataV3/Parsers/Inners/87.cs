namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx26ʺ_entityCastOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityCastOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityCastOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityCastOptionↃ> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_entityCastOption_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺ_entityCastOptionParser.Instance.Parse(input);
if (!_ʺx26ʺ_entityCastOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityCastOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityCastOptionↃ(_ʺx26ʺ_entityCastOption_1.Parsed), _ʺx26ʺ_entityCastOption_1.Remainder);
            }
        }
    }
    
}
