namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_orderbyItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ> Parse(IInput<char>? input)
            {
                var _COMMA_orderbyItem_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_orderbyItemParser.Instance.Parse(input);
if (!_COMMA_orderbyItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ(_COMMA_orderbyItem_1.Parsed), _COMMA_orderbyItem_1.Remainder);
            }
        }
    }
    
}
