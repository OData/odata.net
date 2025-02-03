namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ATParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._AT> Instance { get; } = (_ʺx40ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._AT>(_ʺx25x34x30ʺParser.Instance);
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._AT._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._AT._ʺx40ʺ(_ʺx40ʺ_1);
        }
        
        public static class _ʺx25x34x30ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._AT._ʺx25x34x30ʺ> Instance { get; } = from _ʺx25x34x30ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x34x30ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._AT._ʺx25x34x30ʺ(_ʺx25x34x30ʺ_1);
        }
    }
    
}
