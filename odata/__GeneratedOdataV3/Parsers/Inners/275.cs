namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr> Instance { get; } = (_keyPredicate_꘡singleNavigationExpr꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr>(_collectionPathExprParser.Instance);
        
        public static class _keyPredicate_꘡singleNavigationExpr꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Parse(IInput<char>? input)
                {
                    var _keyPredicate_1 = __GeneratedOdataV3.Parsers.Rules._keyPredicateParser.Instance.Parse(input);
if (!_keyPredicate_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡)!, input);
}

var _singleNavigationExpr_1 = __GeneratedOdataV3.Parsers.Rules._singleNavigationExprParser.Instance.Optional().Parse(_keyPredicate_1.Remainder);
if (!_singleNavigationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡(_keyPredicate_1.Parsed, _singleNavigationExpr_1.Parsed.GetOrElse(null)), _singleNavigationExpr_1.Remainder);
                }
            }
        }
        
        public static class _collectionPathExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr> Parse(IInput<char>? input)
                {
                    var _collectionPathExpr_1 = __GeneratedOdataV3.Parsers.Rules._collectionPathExprParser.Instance.Parse(input);
if (!_collectionPathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr(_collectionPathExpr_1.Parsed), _collectionPathExpr_1.Remainder);
                }
            }
        }
    }
    
}
