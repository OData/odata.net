namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pcharParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._pchar>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._pchar>(_subⲻdelimsParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._pchar>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._pchar>(_ʺx40ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdataV2.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pchar._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdataV2.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pchar._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdataV2.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pchar._subⲻdelims(_subⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pchar._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pchar._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pchar._ʺx40ʺ(_ʺx40ʺ_1);
        }
    }
    
}
