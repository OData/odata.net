namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _firstMemberExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr> Instance { get; } = (_memberExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr>(_inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser.Instance);
        
        public static class _memberExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._memberExpr> Instance { get; } = from _memberExpr_1 in __GeneratedOdataV3.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._memberExpr(_memberExpr_1);
        }
        
        public static class _inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡> Parse(IInput<char>? input)
                {
                    var _inscopeVariableExpr_1 = __GeneratedOdataV3.Parsers.Rules._inscopeVariableExprParser.Instance.Parse(input);
if (!_inscopeVariableExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡)!, input);
}

var _ʺx2Fʺ_memberExpr_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_memberExprParser.Instance.Optional().Parse(_inscopeVariableExpr_1.Remainder);
if (!_ʺx2Fʺ_memberExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._firstMemberExpr._inscopeVariableExpr_꘡ʺx2Fʺ_memberExpr꘡(_inscopeVariableExpr_1.Parsed,  _ʺx2Fʺ_memberExpr_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_memberExpr_1.Remainder);
                }
            }
        }
    }
    
}
