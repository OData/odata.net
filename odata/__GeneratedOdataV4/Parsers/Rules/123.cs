namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _inscopeVariableExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr> Instance { get; } = (_implicitVariableExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr>(_parameterAliasParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr>(_lambdaVariableExprParser.Instance);
        
        public static class _implicitVariableExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr> Parse(IInput<char>? input)
                {
                    var _implicitVariableExpr_1 = __GeneratedOdataV4.Parsers.Rules._implicitVariableExprParser.Instance.Parse(input);
if (!_implicitVariableExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._implicitVariableExpr(_implicitVariableExpr_1.Parsed), _implicitVariableExpr_1.Remainder);
                }
            }
        }
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._parameterAlias> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._parameterAlias>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._parameterAlias> Parse(IInput<char>? input)
                {
                    var _parameterAlias_1 = __GeneratedOdataV4.Parsers.Rules._parameterAliasParser.Instance.Parse(input);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._parameterAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._parameterAlias(_parameterAlias_1.Parsed), _parameterAlias_1.Remainder);
                }
            }
        }
        
        public static class _lambdaVariableExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr> Parse(IInput<char>? input)
                {
                    var _lambdaVariableExpr_1 = __GeneratedOdataV4.Parsers.Rules._lambdaVariableExprParser.Instance.Parse(input);
if (!_lambdaVariableExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._inscopeVariableExpr._lambdaVariableExpr(_lambdaVariableExpr_1.Parsed), _lambdaVariableExpr_1.Remainder);
                }
            }
        }
    }
    
}
