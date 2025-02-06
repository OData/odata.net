namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_computeItemↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_computeItemↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_computeItemↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_computeItemↃ> Parse(IInput<char>? input)
            {
                var _COMMA_computeItem_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_computeItemParser.Instance.Parse(input);
if (!_COMMA_computeItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_computeItemↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_computeItemↃ(_COMMA_computeItem_1.Parsed), _COMMA_computeItem_1.Remainder);
            }
        }
    }
    
}
