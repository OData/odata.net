namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath> Instance { get; } = (_ordinalIndexParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath>(_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡Parser.Instance);
        
        public static class _ordinalIndexParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath._ordinalIndex> Instance { get; } = from _ordinalIndex_1 in __GeneratedOdataV3.Parsers.Rules._ordinalIndexParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._complexColPath._ordinalIndex(_ordinalIndex_1);
        }
        
        public static class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡)!, input);
}

var _countⳆboundOperation_1 = __GeneratedOdataV3.Parsers.Inners._countⳆboundOperationParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
if (!_countⳆboundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._complexColPath._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_꘡countⳆboundOperation꘡(_ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null),  _countⳆboundOperation_1.Parsed.GetOrElse(null)), _countⳆboundOperation_1.Remainder);
                }
            }
        }
    }
    
}
