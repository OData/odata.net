namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _pcharⳆʺx2FʺⳆʺx3FʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ> Instance { get; } = (_pcharParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx2FʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx3FʺParser.Instance);
        
        public static class _pcharParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar> Instance { get; } = from _pchar_1 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance
select new __GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar(_pchar_1);
        }
        
        public static class _ʺx2FʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ(_ʺx2Fʺ_1);
        }
        
        public static class _ʺx3FʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ(_ʺx3Fʺ_1);
        }
    }
    
}
