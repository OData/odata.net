namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue> Instance { get; } = (_ʺx2Fʺ_propertyPathParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_boundOperationParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_refParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue>(_valueParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _propertyPath_1 in __GeneratedOdata.Parsers.Rules._propertyPathParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(_ʺx2Fʺ_1, _propertyPath_1);
        }
        
        public static class _boundOperationParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._boundOperation(_boundOperation_1);
        }
        
        public static class _refParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref> Instance { get; } = from _ref_1 in __GeneratedOdata.Parsers.Rules._refParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ref(_ref_1);
        }
        
        public static class _valueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value> Instance { get; } = from _value_1 in __GeneratedOdata.Parsers.Rules._valueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._value(_value_1);
        }
    }
    
}
