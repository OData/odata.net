namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSPⳆHTABↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABↃ> Parse(IInput<char>? input)
            {
                var _SPⳆHTAB_1 = __GeneratedOdataV3.Parsers.Inners._SPⳆHTABParser.Instance.Parse(input);
if (!_SPⳆHTAB_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABↃ(_SPⳆHTAB_1.Parsed), _SPⳆHTAB_1.Remainder);
            }
        }
    }
    
}
