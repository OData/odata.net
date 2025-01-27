namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColPathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._complexColPath> Instance { get; } = (_ordinalIndexParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._complexColPath>(_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡Parser.Instance);
        
        public static class _ordinalIndexParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._complexColPath._ordinalIndex> Instance { get; } = from _ordinalIndex_1 in __GeneratedOdata.Parsers.Rules._ordinalIndexParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexColPath._ordinalIndex(_ordinalIndex_1);
        }
        
        public static class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _countⳆboundOperation_1 in __GeneratedOdata.Parsers.Inners._countⳆboundOperationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _countⳆboundOperation_1.GetOrElse(null));
        }
    }
    
}
