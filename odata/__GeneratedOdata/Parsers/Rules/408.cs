namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _subⲻdelimsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims> Instance { get; } = (_ʺx24ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._subⲻdelims>(_ʺx26ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._subⲻdelims>(_ʺx27ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._subⲻdelims>(_ʺx3DʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._subⲻdelims>(_otherⲻdelimsParser.Instance);
        
        public static class _ʺx24ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx24ʺ> Instance { get; } = from _ʺx24ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx24ʺ(_ʺx24ʺ_1);
        }
        
        public static class _ʺx26ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx26ʺ> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx26ʺ(_ʺx26ʺ_1);
        }
        
        public static class _ʺx27ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx27ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx27ʺ(_ʺx27ʺ_1);
        }
        
        public static class _ʺx3DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subⲻdelims._ʺx3Dʺ(_ʺx3Dʺ_1);
        }
        
        public static class _otherⲻdelimsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._subⲻdelims._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdata.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subⲻdelims._otherⲻdelims(_otherⲻdelims_1);
        }
    }
    
}
