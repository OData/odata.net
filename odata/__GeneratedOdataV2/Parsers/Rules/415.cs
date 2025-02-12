namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻunescapedParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_pctⲻencodedⲻunescapedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx40ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx3FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped>(_ʺx3DʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdataV2.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedⲻunescapedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._pctⲻencodedⲻunescaped> Instance { get; } = from _pctⲻencodedⲻunescaped_1 in __GeneratedOdataV2.Parsers.Rules._pctⲻencodedⲻunescapedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._pctⲻencodedⲻunescaped(_pctⲻencodedⲻunescaped_1);
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdataV2.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._otherⲻdelims(_otherⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Aʺ.Instance;
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx40ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx40ʺ.Instance;
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx2Fʺ.Instance;
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Fʺ> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3FʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Fʺ.Instance;
        }
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx24ʺ> Instance { get; } = from _ʺx24ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx24ʺ.Instance;
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx27ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx27ʺ.Instance;
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻunescaped._ʺx3Dʺ.Instance;
        }
    }
    
}
