namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _annotationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._annotation> Instance { get; } = from _AT_1 in __GeneratedOdata.Parsers.Rules._ATParser.Instance
from _namespace_ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._namespace_ʺx2EʺParser.Instance.Optional()
from _termName_1 in __GeneratedOdata.Parsers.Rules._termNameParser.Instance
from _ʺx23ʺ_annotationQualifier_1 in __GeneratedOdata.Parsers.Inners._ʺx23ʺ_annotationQualifierParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._annotation(_AT_1, _namespace_ʺx2Eʺ_1.GetOrElse(null), _termName_1, _ʺx23ʺ_annotationQualifier_1.GetOrElse(null));
    }
    
}
