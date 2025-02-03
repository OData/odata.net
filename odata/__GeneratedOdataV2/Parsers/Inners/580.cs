namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_annotationIdentifierↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ> Instance { get; } = from _COMMA_annotationIdentifier_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_annotationIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ(_COMMA_annotationIdentifier_1);
    }
    
}
