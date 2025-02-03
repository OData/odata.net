namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _inscopeVariableExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr> Instance { get; } = (_implicitVariableExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr>(_parameterAliasParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr>(_lambdaVariableExprParser.Instance);
        
        public static class _implicitVariableExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr> Instance { get; } = from _implicitVariableExpr_1 in __GeneratedOdataV3.Parsers.Rules._implicitVariableExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr(_implicitVariableExpr_1);
        }
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdataV3.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._parameterAlias(_parameterAlias_1);
        }
        
        public static class _lambdaVariableExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr> Instance { get; } = from _lambdaVariableExpr_1 in __GeneratedOdataV3.Parsers.Rules._lambdaVariableExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr(_lambdaVariableExpr_1);
        }
    }
    
}
