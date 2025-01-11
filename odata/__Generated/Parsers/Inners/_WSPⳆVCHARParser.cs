namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _WSPⳆVCHARParser
    {
        public static Parser<__Generated.CstNodes.Inners._WSPⳆVCHAR> Instance { get; }
        
        public static class _WSPParser
        {
            public static Parser<__Generated.CstNodes.Inners._WSPⳆVCHAR._WSP> Instance { get; } = from _WSP_1 in __Generated.Parsers.Rules._WSPParser.Instance
select new __Generated.CstNodes.Inners._WSPⳆVCHAR._WSP(_WSP_1);
        }
        
        public static class _VCHARParser
        {
            public static Parser<__Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR> Instance { get; } = from _VCHAR_1 in __Generated.Parsers.Rules._VCHARParser.Instance
select new __Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR(_VCHAR_1);
        }
    }
    
}
