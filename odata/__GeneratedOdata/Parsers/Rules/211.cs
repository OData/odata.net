namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _quotationⲻmarkParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._quotationⲻmark> Instance { get; } = (_DQUOTEParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._quotationⲻmark>(_ʺx25x32x32ʺParser.Instance);
        
        public static class _DQUOTEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._quotationⲻmark._DQUOTE> Instance { get; } = from _DQUOTE_1 in __GeneratedOdata.Parsers.Rules._DQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._quotationⲻmark._DQUOTE(_DQUOTE_1);
        }
        
        public static class _ʺx25x32x32ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ> Instance { get; } = from _ʺx25x32x32ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x32ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ(_ʺx25x32x32ʺ_1);
        }
    }
    
}
