namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_positionLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ> Instance { get; } = from _COMMA_positionLiteral_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_positionLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ(_COMMA_positionLiteral_1);
    }
    
}
