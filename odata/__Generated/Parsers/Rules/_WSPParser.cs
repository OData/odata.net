namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _WSPParser
    {
        public static Parser<__Generated.CstNodes.Rules._WSP> Instance { get; }
        
        public static class _SPParser
        {
            public static Parser<__Generated.CstNodes.Rules._WSP._SP> Instance { get; } = from _SP_1 in __Generated.Parsers.Rules._SPParser.Instance
select new __Generated.CstNodes.Rules._WSP._SP(_SP_1);
        }
        
        public static class _HTABParser
        {
            public static Parser<__Generated.CstNodes.Rules._WSP._HTAB> Instance { get; } = from _HTAB_1 in __Generated.Parsers.Rules._HTABParser.Instance
select new __Generated.CstNodes.Rules._WSP._HTAB(_HTAB_1);
        }
    }
    
}
