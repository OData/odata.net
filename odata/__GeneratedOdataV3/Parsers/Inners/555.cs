namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_ringLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ> Instance { get; } = from _COMMA_ringLiteral_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_ringLiteralParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ(_COMMA_ringLiteral_1);
    }
    
}
