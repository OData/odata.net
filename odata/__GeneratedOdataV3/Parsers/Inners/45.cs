namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue> Instance { get; } = (_ʺx2Fʺ_propertyPathParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_boundOperationParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_refParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_valueParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
from _propertyPath_1 in __GeneratedOdataV3.Parsers.Rules._propertyPathParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(_ʺx2Fʺ_1, _propertyPath_1);
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation(_boundOperation_1);
        }
        
        public static class _refParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref> Instance { get; } = from _ref_1 in __GeneratedOdataV3.Parsers.Rules._refParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref(_ref_1);
        }
        
        public static class _valueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value> Instance { get; } = from _value_1 in __GeneratedOdataV3.Parsers.Rules._valueParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value(_value_1);
        }
    }
    
}
