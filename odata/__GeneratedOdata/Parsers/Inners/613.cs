namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SPⳆHTABParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTAB> Instance { get; } = (_SPParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._SPⳆHTAB>(_HTABParser.Instance);
        
        public static class _SPParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTAB._SP> Instance { get; } = from _SP_1 in __GeneratedOdata.Parsers.Rules._SPParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTAB._SP(_SP_1);
        }
        
        public static class _HTABParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTAB._HTAB> Instance { get; } = from _HTAB_1 in __GeneratedOdata.Parsers.Rules._HTABParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTAB._HTAB(_HTAB_1);
        }
    }
    
}
