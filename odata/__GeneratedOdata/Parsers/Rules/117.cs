namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _firstMemberExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr> Instance { get; } = (_memberExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr>(_inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr> Instance { get; } = _memberExprParser.Instance;

        public static class _memberExprParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr> Instance { get; } = from _memberExpr_1 in __GeneratedOdata.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr(_memberExpr_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr>
            {
                public IOutput<char, _firstMemberExpr._memberExpr> Parse(IInput<char>? input)
                {
                    var _memberExpr_1 = __GeneratedOdata.Parsers.Rules._memberExprParser.Instance.Parse(input);
                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Rules._firstMemberExpr._memberExpr(_memberExpr_1.Parsed),
                        _memberExpr_1.Remainder);
                }
            }
        }
        
        public static class _inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡> Instance { get; } = from _inscopeVariableExpr_1 in __GeneratedOdata.Parsers.Rules._inscopeVariableExprParser.Instance
from _ʺx2Fʺ_memberExpr_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_memberExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡(_inscopeVariableExpr_1, _ʺx2Fʺ_memberExpr_1.GetOrElse(null));
        }
    }
    
}
