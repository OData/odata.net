namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _inscopeVariableExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr> Instance { get; } = (_implicitVariableExprParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr>(_parameterAliasParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr>(_lambdaVariableExprParser.Instance);
        
        public static class _implicitVariableExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr> Instance { get; } = from _implicitVariableExpr_1 in __GeneratedOdata.Parsers.Rules._implicitVariableExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr(_implicitVariableExpr_1);
        }
        
        public static class _parameterAliasParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdata.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._parameterAlias(_parameterAlias_1);
        }
        
        public static class _lambdaVariableExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr> Instance { get; } = from _lambdaVariableExpr_1 in __GeneratedOdata.Parsers.Rules._lambdaVariableExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr(_lambdaVariableExpr_1);
        }
    }
    
}