namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_propertyPathⳆboundOperationParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation> Instance { get; } = (_ʺx2Fʺ_propertyPathParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _propertyPath_1 in __GeneratedOdata.Parsers.Rules._propertyPathParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath(_ʺx2Fʺ_1, _propertyPath_1);
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation(_boundOperation_1);
        }
    }
    
}
