namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded> Instance { get; } = (_ALPHAParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_COMMAParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_ʺx2EʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_pctⲻencodedParser.Instance);
        
        public static class _ALPHAParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA(_ALPHA_1);
        }
        
        public static class _DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT(_DIGIT_1);
        }
        
        public static class _COMMAParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA(_COMMA_1);
        }
        
        public static class _ʺx2EʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ(_ʺx2Eʺ_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded(_pctⲻencoded_1);
        }
    }
    
}
