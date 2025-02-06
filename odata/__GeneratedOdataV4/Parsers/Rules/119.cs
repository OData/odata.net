namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _propertyPathExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr> Parse(IInput<char>? input)
            {
                var _ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Ↄ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡ↃParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr(_ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Ↄ_1.Parsed), _ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Ↄ_1.Remainder);
            }
        }
    }
    
}
