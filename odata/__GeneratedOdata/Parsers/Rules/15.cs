namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _singleNavigationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._singleNavigation> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._singleNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.GetOrElse(null));
    }
    
}
