namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._complexPath> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathⳆboundOperation_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._complexPath(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathⳆboundOperation_1.GetOrElse(null));
    }
    
}
