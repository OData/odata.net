namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._singleNavigation> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._singleNavigation>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._singleNavigation Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._singleNavigation> Parse(IInput<char>? input)
            {

var _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._singleNavigation)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._singleNavigation(null, _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Remainder);
            }
        }
    }
    
}
