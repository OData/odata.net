namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_functionParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_functionParameterↃ> Instance { get; } = from _COMMA_functionParameter_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_functionParameterParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_functionParameterↃ(_COMMA_functionParameter_1);
    }
    
}
