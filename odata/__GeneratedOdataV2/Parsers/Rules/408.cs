namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _subⲻdelimsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims> Instance { get; } = (_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims>(_ʺx26ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims>(_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims>(_ʺx3DʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims>(_otherⲻdelimsParser.Instance);
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx24ʺ> Instance { get; } = from _ʺx24ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx24ʺ.Instance;
        }
        
        public static class _ʺx26ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx26ʺ> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx26ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx26ʺ.Instance;
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx27ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx27ʺ.Instance;
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx3Dʺ> Instance { get; } = from _ʺx3Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._ʺx3Dʺ.Instance;
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdataV2.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._subⲻdelims._otherⲻdelims(_otherⲻdelims_1);
        }
    }
    
}
