namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSEMI_selectOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_selectOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_selectOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_selectOptionↃ> Parse(IInput<char>? input)
            {
                var _SEMI_selectOption_1 = __GeneratedOdataV4.Parsers.Inners._SEMI_selectOptionParser.Instance.Parse(input);
if (!_SEMI_selectOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_selectOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤSEMI_selectOptionↃ(_SEMI_selectOption_1.Parsed), _SEMI_selectOption_1.Remainder);
            }
        }
    }
    
}
