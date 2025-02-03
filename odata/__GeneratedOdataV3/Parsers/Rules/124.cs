namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _implicitVariableExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr> Instance { get; } = (_ʺx24x69x74ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr>(_ʺx24x74x68x69x73ʺParser.Instance);
        
        public static class _ʺx24x69x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ> Instance { get; } = from _ʺx24x69x74ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx24x69x74ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ(_ʺx24x69x74ʺ_1);
        }
        
        public static class _ʺx24x74x68x69x73ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ> Instance { get; } = from _ʺx24x74x68x69x73ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx24x74x68x69x73ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ(_ʺx24x74x68x69x73ʺ_1);
        }
    }
    
}
