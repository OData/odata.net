namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleNavigation> Parse(IInput<char>? input)
            {

var _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavigation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleNavigation(null, _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Remainder);
            }
        }
    }
    
}
