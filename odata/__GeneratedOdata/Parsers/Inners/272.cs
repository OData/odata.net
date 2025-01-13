namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx23ʺ_annotationQualifierParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx23ʺ_annotationQualifier> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx23ʺParser.Instance
from _annotationQualifier_1 in __GeneratedOdata.Parsers.Rules._annotationQualifierParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx23ʺ_annotationQualifier(_ʺx23ʺ_1, _annotationQualifier_1);
    }
    
}
