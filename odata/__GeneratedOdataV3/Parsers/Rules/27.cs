namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundOperationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundOperation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundOperation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundOperation> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundOperation)!, input);
}

var _ⲤboundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤboundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensↃParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_ⲤboundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundOperation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundOperation(_ʺx2Fʺ_1.Parsed, _ⲤboundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensↃ_1.Parsed), _ⲤboundActionCallⳆboundEntityColFunctionCall_꘡collectionNavigation꘡ⳆboundEntityFunctionCall_꘡singleNavigation꘡ⳆboundComplexColFunctionCall_꘡complexColPath꘡ⳆboundComplexFunctionCall_꘡complexPath꘡ⳆboundPrimitiveColFunctionCall_꘡primitiveColPath꘡ⳆboundPrimitiveFunctionCall_꘡primitivePath꘡ⳆboundFunctionCallNoParensↃ_1.Remainder);
            }
        }
    }
    
}
