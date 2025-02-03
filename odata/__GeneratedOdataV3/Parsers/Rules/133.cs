namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._functionExpr> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExpr)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExpr)!, input);
}

var _ⲤentityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Ↄ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤentityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡ↃParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_ⲤentityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._functionExpr(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _ⲤentityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Ↄ_1.Parsed), _ⲤentityColFunction_functionExprParameters_꘡collectionNavigationExpr꘡ⳆentityFunction_functionExprParameters_꘡singleNavigationExpr꘡ⳆcomplexColFunction_functionExprParameters_꘡complexColPathExpr꘡ⳆcomplexFunction_functionExprParameters_꘡complexPathExpr꘡ⳆprimitiveColFunction_functionExprParameters_꘡collectionPathExpr꘡ⳆprimitiveFunction_functionExprParameters_꘡primitivePathExpr꘡Ↄ_1.Remainder);
            }
        }
    }
    
}
