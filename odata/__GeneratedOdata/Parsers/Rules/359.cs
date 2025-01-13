namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _annotationsListParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._annotationsList> Instance { get; } = from _annotationIdentifier_1 in __GeneratedOdata.Parsers.Rules._annotationIdentifierParser.Instance
from _ⲤCOMMA_annotationIdentifierↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_annotationIdentifierↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._annotationsList(_annotationIdentifier_1, _ⲤCOMMA_annotationIdentifierↃ_1);
    }
    
}
