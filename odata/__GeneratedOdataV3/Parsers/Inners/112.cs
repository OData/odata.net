namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSEMI_expandCountOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandCountOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandCountOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandCountOptionↃ> Parse(IInput<char>? input)
            {
                var _SEMI_expandCountOption_1 = __GeneratedOdataV3.Parsers.Inners._SEMI_expandCountOptionParser.Instance.Parse(input);
if (!_SEMI_expandCountOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandCountOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_expandCountOptionↃ(_SEMI_expandCountOption_1.Parsed), _SEMI_expandCountOption_1.Remainder);
            }
        }
    }
    
}
