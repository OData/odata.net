namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _firstMemberExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._firstMemberExpr> Instance { get; } = (_memberExprParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._firstMemberExpr>(_inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser.Instance);
        
        public static class _memberExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr> Instance { get; } = from _memberExpr_1 in __GeneratedOdata.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr(_memberExpr_1);
        }
        
        public static class _inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡> Instance { get; } = from _inscopeVariableExpr_1 in __GeneratedOdata.Parsers.Rules._inscopeVariableExprParser.Instance
from _ʺx2Fʺ_memberExpr_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_memberExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡(_inscopeVariableExpr_1, _ʺx2Fʺ_memberExpr_1.GetOrElse(null));
        }
    }
    
}
