namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_annotationIdentifierParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_annotationIdentifier> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _annotationIdentifier_1 in __GeneratedOdata.Parsers.Rules._annotationIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_annotationIdentifier(_COMMA_1, _annotationIdentifier_1);
    }
    
}
