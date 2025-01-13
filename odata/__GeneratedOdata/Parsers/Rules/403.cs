namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _pcharParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._pchar> Instance { get; } = (_unreservedParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pchar>(_pctⲻencodedParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pchar>(_subⲻdelimsParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pchar>(_ʺx3AʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pchar>(_ʺx40ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pchar._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pchar._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pchar._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pchar._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pchar._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdata.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pchar._subⲻdelims(_subⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pchar._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pchar._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx40ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pchar._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pchar._ʺx40ʺ(_ʺx40ʺ_1);
        }
    }
    
}
