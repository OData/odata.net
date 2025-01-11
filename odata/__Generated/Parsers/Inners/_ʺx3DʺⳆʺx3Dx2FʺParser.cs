namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx3DʺⳆʺx3Dx2FʺParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ> Instance { get; } = (_ʺx3DʺParser.Instance).Or<__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ>(_ʺx3Dx2FʺParser.Instance);
        
        public static class _ʺx3DʺParser
        {
            public static Parser<__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __Generated.Parsers.Inners._ʺx3DʺParser.Instance
select new __Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ(_ʺx3Dʺ_1);
        }
        
        public static class _ʺx3Dx2FʺParser
        {
            public static Parser<__Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ> Instance { get; } = from _ʺx3Dx2Fʺ_1 in __Generated.Parsers.Inners._ʺx3Dx2FʺParser.Instance
select new __Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ(_ʺx3Dx2Fʺ_1);
        }
    }
    
}
