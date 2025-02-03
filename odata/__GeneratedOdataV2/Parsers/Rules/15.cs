namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleNavigation> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._singleNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.GetOrElse(null));
    }
    
}
