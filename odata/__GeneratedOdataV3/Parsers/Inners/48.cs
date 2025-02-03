namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_propertyPathⳆboundOperationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation> Instance { get; } = (_ʺx2Fʺ_propertyPathParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
from _propertyPath_1 in __GeneratedOdataV3.Parsers.Rules._propertyPathParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath(_ʺx2Fʺ_1, _propertyPath_1);
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation(_boundOperation_1);
        }
    }
    
}
