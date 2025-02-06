namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx26ʺ_queryOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_queryOption_1 = __GeneratedOdataV4.Parsers.Inners._ʺx26ʺ_queryOptionParser.Instance.Parse(input);
if (!_ʺx26ʺ_queryOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ(_ʺx26ʺ_queryOption_1.Parsed), _ʺx26ʺ_queryOption_1.Remainder);
            }
        }
    }
    
}
