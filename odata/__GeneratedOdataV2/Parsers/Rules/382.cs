namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._CLOSE> Instance { get; } = (_ʺx29ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._CLOSE>(_ʺx25x32x39ʺParser.Instance);
        
        public static class _ʺx29ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._CLOSE._ʺx29ʺ> Instance { get; } = from _ʺx29ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx29ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._CLOSE._ʺx29ʺ(_ʺx29ʺ_1);
        }
        
        public static class _ʺx25x32x39ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ> Instance { get; } = from _ʺx25x32x39ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x32x39ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ(_ʺx25x32x39ʺ_1);
        }
    }
    
}
