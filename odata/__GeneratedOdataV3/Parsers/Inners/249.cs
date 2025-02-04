namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_selectListItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectListItemↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectListItemↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectListItemↃ> Parse(IInput<char>? input)
            {
                var _COMMA_selectListItem_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_selectListItemParser.Instance.Parse(input);
if (!_COMMA_selectListItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectListItemↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_selectListItemↃ(_COMMA_selectListItem_1.Parsed), _COMMA_selectListItem_1.Remainder);
            }
        }
    }
    
}
