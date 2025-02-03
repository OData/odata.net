namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_annotationQualifierParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx23ʺ_annotationQualifier> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx23ʺParser.Instance
from _annotationQualifier_1 in __GeneratedOdataV2.Parsers.Rules._annotationQualifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx23ʺ_annotationQualifier(_ʺx23ʺ_1, _annotationQualifier_1);
    }
    
}
