namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx26ʺ_entityIdOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityIdOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityIdOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityIdOptionↃ> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_entityIdOption_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺ_entityIdOptionParser.Instance.Parse(input);
if (!_ʺx26ʺ_entityIdOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityIdOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_entityIdOptionↃ(_ʺx26ʺ_entityIdOption_1.Parsed), _ʺx26ʺ_entityIdOption_1.Remainder);
            }
        }
    }
    
}
