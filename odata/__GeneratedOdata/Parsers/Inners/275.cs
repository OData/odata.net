namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr> Instance { get; } = (_keyPredicate_꘡singleNavigationExpr꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr>(_collectionPathExprParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr> Instance { get; } = _keyPredicate_꘡singleNavigationExpr꘡Parser.Instance;

        public static class _keyPredicate_꘡singleNavigationExpr꘡Parser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Instance { get; } = from _keyPredicate_1 in __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance
from _singleNavigationExpr_1 in __GeneratedOdata.Parsers.Rules._singleNavigationExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡(_keyPredicate_1, _singleNavigationExpr_1.GetOrElse(null));
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡>
            {
                public IOutput<char, _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Parse(IInput<char>? input)
                {
                    var _keyPredicate_1 = __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance.Parse(input);
                    if (!_keyPredicate_1.Success)
                    {
                        return Output.Create(
                            false,
                            default(_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡)!,
                            input);
                    }

                    var _singleNavigationExpr_1 = __GeneratedOdata.Parsers.Rules._singleNavigationExprParser.Instance.Optional().Parse(_keyPredicate_1.Remainder);
                    if (!_singleNavigationExpr_1.Success)
                    {
                        return Output.Create(
                            false,
                            default(_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡)!,
                            input);
                    }

                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡(_keyPredicate_1.Parsed, _singleNavigationExpr_1.Parsed.GetOrElse(null)),
                        _singleNavigationExpr_1.Remainder);
                }
            }
        }
        
        public static class _collectionPathExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr> Instance { get; } = from _collectionPathExpr_1 in __GeneratedOdata.Parsers.Rules._collectionPathExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr(_collectionPathExpr_1);
        }
    }
    
}
