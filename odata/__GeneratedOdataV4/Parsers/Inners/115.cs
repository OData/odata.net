namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSEMI_expandOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_expandOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_expandOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_expandOptionↃ> Parse(IInput<char>? input)
            {
                var _SEMI_expandOption_1 = __GeneratedOdataV4.Parsers.Inners._SEMI_expandOptionParser.Instance.Parse(input);
if (!_SEMI_expandOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_expandOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_expandOptionↃ(_SEMI_expandOption_1.Parsed), _SEMI_expandOption_1.Remainder);
            }
        }
    }
    
}
