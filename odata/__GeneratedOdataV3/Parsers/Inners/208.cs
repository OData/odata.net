namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_selectItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectItemↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectItemↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectItemↃ> Parse(IInput<char>? input)
            {
                var _COMMA_selectItem_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_selectItemParser.Instance.Parse(input);
if (!_COMMA_selectItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectItemↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectItemↃ(_COMMA_selectItem_1.Parsed), _COMMA_selectItem_1.Remainder);
            }
        }
    }
    
}
