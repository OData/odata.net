namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_annotationIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_annotationIdentifier> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _annotationIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._annotationIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_annotationIdentifier(_COMMA_1, _annotationIdentifier_1);
    }
    
}
