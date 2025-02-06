namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigationExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigationExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigationExpr> Parse(IInput<char>? input)
            {

var _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1 = __GeneratedOdataV4.Parsers.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser.Instance.Optional().Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionNavigationExpr(null, _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.Parsed.GetOrElse(null)), _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.Remainder);
            }
        }
    }
    
}
