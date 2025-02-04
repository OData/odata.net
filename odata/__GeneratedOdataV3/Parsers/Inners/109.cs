namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSEMI_expandRefOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandRefOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandRefOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandRefOptionↃ> Parse(IInput<char>? input)
            {
                var _SEMI_expandRefOption_1 = __GeneratedOdataV3.Parsers.Inners._SEMI_expandRefOptionParser.Instance.Parse(input);
if (!_SEMI_expandRefOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandRefOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandRefOptionↃ(_SEMI_expandRefOption_1.Parsed), _SEMI_expandRefOption_1.Remainder);
            }
        }
    }
    
}
