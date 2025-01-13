namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _unreservedⳆpctⲻencodedⳆsubⲻdelimsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims> Instance { get; } = (_unreservedParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_pctⲻencodedParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_subⲻdelimsParser.Instance);
        
        public static class _unreservedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdata.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims(_subⲻdelims_1);
        }
    }
    
}
