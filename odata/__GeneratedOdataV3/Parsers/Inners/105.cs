namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_expandItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ> Parse(IInput<char>? input)
            {
                var _COMMA_expandItem_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_expandItemParser.Instance.Parse(input);
if (!_COMMA_expandItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ(_COMMA_expandItem_1.Parsed), _COMMA_expandItem_1.Remainder);
            }
        }
    }
    
}
