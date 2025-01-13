namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _implicitVariableExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._implicitVariableExpr> Instance { get; } = (_ʺx24x69x74ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._implicitVariableExpr>(_ʺx24x74x68x69x73ʺParser.Instance);
        
        public static class _ʺx24x69x74ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ> Instance { get; } = from _ʺx24x69x74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x69x74ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._implicitVariableExpr._ʺx24x69x74ʺ(_ʺx24x69x74ʺ_1);
        }
        
        public static class _ʺx24x74x68x69x73ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ> Instance { get; } = from _ʺx24x74x68x69x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x74x68x69x73ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._implicitVariableExpr._ʺx24x74x68x69x73ʺ(_ʺx24x74x68x69x73ʺ_1);
        }
    }
    
}
