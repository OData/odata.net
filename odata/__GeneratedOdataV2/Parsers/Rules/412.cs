namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx40ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3DʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdataV2.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdataV2.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdataV2.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims(_otherⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ(_ʺx40ʺ_1);
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ(_ʺx2Fʺ_1);
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ(_ʺx3Fʺ_1);
        }
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ> Instance { get; } = from _ʺx24ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ(_ʺx24ʺ_1);
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx27ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ(_ʺx27ʺ_1);
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3DʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ(_ʺx3Dʺ_1);
        }
    }
    
}
