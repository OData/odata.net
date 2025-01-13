namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _pcharⲻnoⲻSQUOTEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE> Instance { get; } = (_unreservedParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_pctⲻencodedⲻnoⲻSQUOTEParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_otherⲻdelimsParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx24ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx26ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx3DʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx3AʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx40ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedⲻnoⲻSQUOTEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE> Instance { get; } = from _pctⲻencodedⲻnoⲻSQUOTE_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedⲻnoⲻSQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE(_pctⲻencodedⲻnoⲻSQUOTE_1);
        }
        
        public static class _otherⲻdelimsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdata.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims(_otherⲻdelims_1);
        }
        
        public static class _ʺx24ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ> Instance { get; } = from _ʺx24ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ(_ʺx24ʺ_1);
        }
        
        public static class _ʺx26ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ(_ʺx26ʺ_1);
        }
        
        public static class _ʺx3DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ(_ʺx3Dʺ_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx40ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ(_ʺx40ʺ_1);
        }
    }
    
}
