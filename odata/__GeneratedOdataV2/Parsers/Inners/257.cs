namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr> Instance { get; } = (_primitiveLiteralParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_arrayOrObjectParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_rootExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_firstMemberExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_functionExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_negateExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_methodCallExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_parenExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_listExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_castExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_isofExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_notExprParser.Instance);
        
        public static class _primitiveLiteralParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral(_primitiveLiteral_1);
        }
        
        public static class _arrayOrObjectParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject> Instance { get; } = from _arrayOrObject_1 in __GeneratedOdataV2.Parsers.Rules._arrayOrObjectParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject(_arrayOrObject_1);
        }
        
        public static class _rootExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr> Instance { get; } = from _rootExpr_1 in __GeneratedOdataV2.Parsers.Rules._rootExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr(_rootExpr_1);
        }
        
        public static class _firstMemberExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr> Instance { get; } = from _firstMemberExpr_1 in __GeneratedOdataV2.Parsers.Rules._firstMemberExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr(_firstMemberExpr_1);
        }
        
        public static class _functionExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr> Instance { get; } = from _functionExpr_1 in __GeneratedOdataV2.Parsers.Rules._functionExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr(_functionExpr_1);
        }
        
        public static class _negateExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr> Instance { get; } = from _negateExpr_1 in __GeneratedOdataV2.Parsers.Rules._negateExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr(_negateExpr_1);
        }
        
        public static class _methodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr> Instance { get; } = from _methodCallExpr_1 in __GeneratedOdataV2.Parsers.Rules._methodCallExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr(_methodCallExpr_1);
        }
        
        public static class _parenExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr> Instance { get; } = from _parenExpr_1 in __GeneratedOdataV2.Parsers.Rules._parenExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr(_parenExpr_1);
        }
        
        public static class _listExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr> Instance { get; } = from _listExpr_1 in __GeneratedOdataV2.Parsers.Rules._listExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr(_listExpr_1);
        }
        
        public static class _castExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr> Instance { get; } = from _castExpr_1 in __GeneratedOdataV2.Parsers.Rules._castExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr(_castExpr_1);
        }
        
        public static class _isofExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr> Instance { get; } = from _isofExpr_1 in __GeneratedOdataV2.Parsers.Rules._isofExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr(_isofExpr_1);
        }
        
        public static class _notExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr> Instance { get; } = from _notExpr_1 in __GeneratedOdataV2.Parsers.Rules._notExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr(_notExpr_1);
        }
    }
    
}
