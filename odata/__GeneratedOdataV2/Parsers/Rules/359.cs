namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationsListParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._annotationsList> Instance { get; } = from _annotationIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._annotationIdentifierParser.Instance
from _ⲤCOMMA_annotationIdentifierↃ_1 in Inners._ⲤCOMMA_annotationIdentifierↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._annotationsList(_annotationIdentifier_1, _ⲤCOMMA_annotationIdentifierↃ_1);
    }
    
}
