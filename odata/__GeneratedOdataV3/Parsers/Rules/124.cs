namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _implicitVariableExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr> Instance { get; } = (_ʺx24x69x74ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr>(_ʺx24x74x68x69x73ʺParser.Instance);
        
        public static class _ʺx24x69x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x69x74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x69x74ʺParser.Instance.Parse(input);
if (!_ʺx24x69x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ.Instance, _ʺx24x69x74ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x74x68x69x73ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x74x68x69x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x74x68x69x73ʺParser.Instance.Parse(input);
if (!_ʺx24x74x68x69x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ.Instance, _ʺx24x74x68x69x73ʺ_1.Remainder);
                }
            }
        }
    }
    
}
