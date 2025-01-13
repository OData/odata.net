namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ> Instance { get; } = (_ALPHAParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2BʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2DʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2EʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA(_ALPHA_1);
        }
        
        public static class _DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT(_DIGIT_1);
        }
        
        public static class _ʺx2BʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ> Instance { get; } = from _ʺx2Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2BʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ(_ʺx2Bʺ_1);
        }
        
        public static class _ʺx2DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
        
        public static class _ʺx2EʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ(_ʺx2Eʺ_1);
        }
    }
    
}
